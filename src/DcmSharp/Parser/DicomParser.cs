using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Text;
using DcmSharp.Memory;
using Microsoft.Extensions.Logging;

namespace DcmSharp.Parser;

public interface IDicomParser
{
    Task<ReadOnlyDicomDataset> ParseReadOnlyAsync(FileInfo file, CancellationToken cancellationToken = default);

    Task<ReadOnlyDicomDataset> ParseReadOnlyAsync(FileInfo file, DicomParserOptions options,
        CancellationToken cancellationToken = default);

    Task ParseAsync(FileInfo file, IDicomDatasetHandler handler, DicomParserOptions? options = null,
        CancellationToken cancellationToken = default);
}

internal sealed class DicomParser : IDicomParser
{
    private static readonly ArrayPool<byte> _shortArrayPool = ArrayPool<byte>.Shared;
    private static readonly ArrayPool<byte> _longArrayPool = ArrayPool<byte>.Create(25 * 1024 * 1024, 32);

    private static readonly DicomItemDictionaryPool _largeDicomItemDictionaryPool =
        new DicomItemDictionaryPool(maxPoolSize: 64);

    private static readonly DicomItemDictionaryPool _smallDicomItemDictionaryPool =
        new DicomItemDictionaryPool(maxPoolSize: 256);

    private static readonly DicomMemoriesPool _memoriesPool = new DicomMemoriesPool(1024, 32);

    private static readonly FileStreamOptions _fileStreamOptions = new FileStreamOptions
    {
        Access = FileAccess.Read, Options = FileOptions.SequentialScan, Share = FileShare.Read, Mode = FileMode.Open
    };

    private readonly ILogger<DicomParser> _logger;
    private readonly DicomValueParser _valueParser;

    private const int BufferSize = 1024 * 1024;
    private const int MinimumMemoryBlockSize = 16384;
    private const int MaxArrayLength = 2_147_483_591;
    private const uint UndefinedLength = 0xffffffff;
    private const ushort FileMetaInformationGroup = 0x0002;
    private const ushort GroupLengthElement = 0x0000;

    private static readonly byte[] _implicitVRLittleEndianBytes = "1.2.840.10008.1.2"u8.ToArray() switch
    {
        { } even when even.Length % 2 == 0 => even,
        { } uneven => [..uneven, 0],
        _ => throw new UnreachableException(),
    };

    private const ushort ItemGroup = 0xFFFE;
    private const ushort Item = 0xE000;
    private const ushort ItemDelimitationItem = 0xE00D;
    private const ushort SequenceDelimitationItem = 0xE0DD;

    public DicomParser(ILogger<DicomParser> logger, DicomValueParser valueParser)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _valueParser = valueParser ?? throw new ArgumentNullException(nameof(valueParser));
    }

    public Task<ReadOnlyDicomDataset> ParseReadOnlyAsync(FileInfo file, CancellationToken cancellationToken = default)
    {
        return ParseReadOnlyAsync(file, DicomParserOptions.Default, cancellationToken);
    }

    [SuppressMessage("Usage", "MA0004:Use Task.ConfigureAwait")]
    public async Task<ReadOnlyDicomDataset> ParseReadOnlyAsync(FileInfo file, DicomParserOptions options,
        CancellationToken cancellationToken = default)
    {
        var memories = new DicomMemories(_memoriesPool);
        var handler = new ReadOnlyDicomDatasetHandler(
            _largeDicomItemDictionaryPool, _smallDicomItemDictionaryPool, memories, _valueParser);
        var pipe = new Pipe();
        var fillPipeTask = FillPipeAsync(file, pipe.Writer, cancellationToken);
        var readPipeTask = ReadPipeAsync(pipe.Reader, handler, memories, options, cancellationToken);
        await Task.WhenAll(fillPipeTask, readPipeTask);
        return handler.Result;
    }

    [SuppressMessage("Usage", "MA0004:Use Task.ConfigureAwait")]
    public async Task ParseAsync(FileInfo file, IDicomDatasetHandler handler, DicomParserOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Memory is allocated from the pool during parsing and slices are passed to the handler callbacks.
        // Per IDicomDatasetHandler contract, rawValue memory is only valid during the callback.
        // We dispose the pool memory here so that rented arrays are returned after all callbacks fire.
        var memories = new DicomMemories(_memoriesPool);
        try
        {
            var pipe = new Pipe();
            var fillPipeTask = FillPipeAsync(file, pipe.Writer, cancellationToken);
            var readPipeTask = ReadPipeAsync(pipe.Reader, handler, memories, options ?? DicomParserOptions.Default, cancellationToken);
            await Task.WhenAll(fillPipeTask, readPipeTask);
        }
        finally
        {
            memories.Dispose();
        }
    }

    private async Task FillPipeAsync(FileInfo file, PipeWriter pipeWriter,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var fileStream = file.Open(_fileStreamOptions);
            long fileLength = fileStream.Length;
            int bufferSize = (int)Math.Min(BufferSize, fileLength);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Memory<byte> memory = pipeWriter.GetMemory(bufferSize);
                int bytesRead = fileStream.Read(memory.Span);

                if (bytesRead == 0)
                    break;

                pipeWriter.Advance(bytesRead);
                FlushResult result = await pipeWriter.FlushAsync(cancellationToken);

                if (result.IsCompleted || result.IsCanceled)
                    break;
            }
        }
        finally
        {
            await pipeWriter.CompleteAsync();
        }
    }

    private async Task ReadPipeAsync(PipeReader reader, IDicomDatasetHandler handler, DicomMemories memories,
        DicomParserOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        ReadResult result = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        if (result.Buffer.Length < 132)
            throw new DicomException("DICOM file is too small to be valid, expected at least 132 bytes");

        EnsureValidPreamble(result.Buffer.Slice(128, 4));
        reader.AdvanceTo(result.Buffer.GetPosition(132));
        long position = 132;

        var state = new DicomParseState
        {
            Handler = handler,
            Memories = memories,
            Logger = _logger,
            ValueParser = _valueParser,
            Position = position,
        };

        try
        {
            while (true)
            {
                result = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);

                if (result.Buffer.Length == 0)
                    break;

                ReadOnlySequence<byte> sequence = result.Buffer;
                DicomByteBuffer buffer = new DicomByteBuffer(result.Buffer);
                long originalPosition = state.Position;
                Parse(ref buffer, ref state, options, cancellationToken);
                long parsed = state.Position - originalPosition;
                if (parsed == 0)
                {
                    if (result.IsCompleted)
                        throw new DicomException(
                            $"DICOM file could not be parsed completely, parsing stopped at position {position}");
                }
                else
                {
                    position += parsed;
                    sequence = sequence.Slice(parsed);
                }

                reader.AdvanceTo(sequence.Start, sequence.End);

                if (result.IsCompleted || state.IsStopped)
                    break;
            }
        }
        finally
        {
            await reader.CompleteAsync().ConfigureAwait(false);
        }
    }

    static void EnsureValidPreamble(ReadOnlySequence<byte> buffer)
    {
        if (buffer.IsSingleSegment)
        {
            var bufferSpan = buffer.FirstSpan;
            if (bufferSpan[0] != 'D' || bufferSpan[1] != 'I' || bufferSpan[2] != 'C' || bufferSpan[3] != 'M')
                throw new DicomException(
                    "This file does not look like a DICOM file, expected 'DICM' at position 128-132");
            return;
        }

        var bufferReader = new SequenceReader<byte>(buffer);
        if (!bufferReader.TryRead(out byte b)
            || b != 'D'
            || !bufferReader.TryRead(out b)
            || b != 'I'
            || !bufferReader.TryRead(out b)
            || b != 'C'
            || !bufferReader.TryRead(out b)
            || b != 'M')
        {
            throw new DicomException(
                "This file does not look like a DICOM file, expected 'DICM' at position 128-132");
        }
    }

    static void Parse(ref DicomByteBuffer buffer, ref DicomParseState state, DicomParserOptions options,
        CancellationToken cancellationToken)
    {
        while (!buffer.IsEmpty)
        {
            cancellationToken.ThrowIfCancellationRequested();

            switch (state.ParseStage)
            {
                case DicomParseStage.ParseGroup:
                    if (!TryParseGroup(ref buffer, ref state))
                        return;
                    goto case DicomParseStage.ParseElement;
                case DicomParseStage.ParseElement:
                    if (!TryParseElement(ref buffer, ref state))
                        return;
                    if (state.ParseStage == DicomParseStage.ParseLength)
                        goto case DicomParseStage.ParseLength;
                    goto case DicomParseStage.ParseVR;
                case DicomParseStage.ParseVR:
                    if (!TryParseVr(ref buffer, ref state))
                        return;
                    goto case DicomParseStage.ParseLength;
                case DicomParseStage.ParseLength:
                    if (!TryParseLength(ref buffer, ref state))
                        return;
                    if (state.ParseStage == DicomParseStage.ParseGroup)
                        goto case DicomParseStage.ParseGroup;
                    goto case DicomParseStage.ParseValue;
                case DicomParseStage.ParseValue:
                    if (!TryParseValue(ref buffer, ref state))
                        return;
                    break;
                default:
                    throw new DicomException("Unknown parse stage: " + state.ParseStage);
            }

            // Check if we need to stop parsing
            if (options.StopParsing is { } stopParsing
                && stopParsing.Depth == state.CurrentSequences.Count
                && stopParsing.Group <= state.CurrentGroupNumber
                && stopParsing.Element <= state.CurrentElementNumber)
            {
                state.IsStopped = true;

                // Close open sequences so the handler can finalize them
                while (state.CurrentSequence is not null)
                {
                    if (state.CurrentSequenceItem is not null)
                    {
                        state.Handler.OnSequenceItemEnd();
                        state.CurrentSequenceItem = null;
                    }

                    state.Handler.OnSequenceEnd();
                    state.CurrentSequence = state.CurrentSequences.TryPop(out var parentSeq) ? parentSeq : null;
                    state.CurrentSequenceItem = state.CurrentSequenceItems.TryPop(out var parentItem) ? parentItem : null;
                }

                return;
            }

            if (state.CurrentSequence is not null)
            {
                // Handle sequence item with explicit length
                if (state.CurrentSequenceItem is { } currentSequenceItem
                    && currentSequenceItem.EndPosition is { } sequenceItemEndPosition
                    && state.Position >= sequenceItemEndPosition)
                {
                    state.Handler.OnSequenceItemEnd();
                    state.CurrentSequenceItem = null;
                    state.ParseStage = DicomParseStage.ParseGroup;
                }

                // Handle sequence with explicit length
                if (state.CurrentSequence is { } currentSequence
                    && currentSequence.EndPosition is { } currentSequenceEndPosition
                    && state.Position >= currentSequenceEndPosition)
                {
                    state.Handler.OnSequenceEnd();
                    state.CurrentSequence = state.CurrentSequences.TryPop(out var parentSeq) ? parentSeq : null;
                    state.CurrentSequenceItem = state.CurrentSequenceItems.TryPop(out var parentItem) ? parentItem : null;
                    state.ParseStage = DicomParseStage.ParseGroup;
                }
            }
        }
    }

    static bool TryParseGroup(ref DicomByteBuffer buffer, ref DicomParseState state)
    {
        if (!buffer.TryReadShort(ref state.Position, out state.ShortHolder))
            return false;

        state.CurrentGroupNumber = (ushort)state.ShortHolder;
        state.ParseStage = DicomParseStage.ParseElement;
        return true;
    }

    static bool TryParseElement(ref DicomByteBuffer buffer, ref DicomParseState state)
    {
        if (!buffer.TryReadShort(ref state.Position, out state.ShortHolder))
            return false;

        state.CurrentElementNumber = (ushort)state.ShortHolder;

        if (state.CurrentGroupNumber > FileMetaInformationGroup)
        {
            if (state is { IsExplicitVR: true, SetToImplicitVrAfterFileMetaInfo: true })
                state.IsExplicitVR = false;

            if (state.CurrentGroupNumber is ItemGroup &&
                state.CurrentElementNumber is Item or ItemDelimitationItem or SequenceDelimitationItem)
            {
                state.ParseStage = DicomParseStage.ParseLength;
                return true;
            }

            if (!state.IsExplicitVR)
            {
                state.CurrentVr = DicomTagsIndex.TryLookup(state.CurrentGroupNumber, state.CurrentElementNumber,
                    out var dicomTag)
                    ? dicomTag.ValueRepresentation
                    : DicomVR.UN;

                if (state is { CurrentVr: DicomVR.UN, CurrentElementNumber: GroupLengthElement })
                    state.CurrentVr = DicomVR.UL;

                state.ParseStage = DicomParseStage.ParseLength;
                return true;
            }
        }

        state.ParseStage = DicomParseStage.ParseVR;
        return true;
    }

    static bool TryParseVr(ref DicomByteBuffer buffer, ref DicomParseState state)
    {
        if (!buffer.TryReadVr(ref state.Position, out byte b1, out byte b2))
            return false;

        if (!DicomVRParser.TryParse(b1, b2, out DicomVR? parsedVr))
            throw new DicomException(
                $"Invalid DICOM file, could not parse ({b1:x4},{b2:x4}) to a known DICOM VR");

        state.CurrentVr = parsedVr.Value;
        state.ParseStage = DicomParseStage.ParseLength;
        return true;
    }

    static bool TryParseLength(ref DicomByteBuffer buffer, ref DicomParseState state)
    {
        if (state.CurrentGroupNumber is ItemGroup)
        {
            if (!buffer.TryReadInt(ref state.Position, out state.IntHolder))
                return false;

            uint rawLongValueLength = (uint)state.IntHolder;

            if (state.CurrentSequence is not null)
            {
                if (state.CurrentElementNumber is Item)
                {
                    if (state.CurrentSequenceItem is not null)
                        throw new DicomException(
                            $"Encountered a DICOM sequence item ({state.CurrentGroupNumber:x4},{state.CurrentElementNumber:x4}) without a preceding DICOM delimitation item");

                    long? sequenceItemLength = rawLongValueLength == UndefinedLength ? null : rawLongValueLength;
                    long? sequenceItemEndPosition = state.Position + sequenceItemLength;
                    state.CurrentSequenceItem = new SequencePosition(sequenceItemEndPosition);
                    state.Handler.OnSequenceItemStart();

                    state.ParseStage = DicomParseStage.ParseGroup;
                    return true;
                }

                if (state.CurrentElementNumber is ItemDelimitationItem)
                {
                    if (state.CurrentSequenceItem is null)
                        throw new DicomException(
                            "Encountered a DICOM item delimitation item without a preceding DICOM item");

                    state.Handler.OnSequenceItemEnd();
                    state.CurrentSequenceItem = null;
                    state.ParseStage = DicomParseStage.ParseGroup;
                    return true;
                }

                if (state.CurrentElementNumber is SequenceDelimitationItem)
                {
                    state.Handler.OnSequenceEnd();
                    state.CurrentSequence = state.CurrentSequences.TryPop(out var parentSeq) ? parentSeq : null;
                    state.CurrentSequenceItem = state.CurrentSequenceItems.TryPop(out var parentItem) ? parentItem : null;
                    state.ParseStage = DicomParseStage.ParseGroup;
                    return true;
                }
            }
            else if (state.IsParsingFragments)
            {
                if (state.CurrentElementNumber is Item)
                {
                    if (rawLongValueLength > MaxArrayLength)
                        throw new NotSupportedException(
                            "DcmParser does not support DICOM files with values larger than 2 GB yet");

                    state.LongValueLength = (int)rawLongValueLength;
                    state.ShortValueLength = null;
                    state.ParseStage = DicomParseStage.ParseValue;
                    return true;
                }

                if (state.CurrentElementNumber is SequenceDelimitationItem)
                {
                    state.Handler.OnFragmentsEnd();
                    state.IsParsingFragments = false;
                    state.ParseStage = DicomParseStage.ParseGroup;
                    return true;
                }

                throw new DicomException(
                    "Expected one of Item or SequenceDelimitationItem while parsing fragment sequence");
            }
        }

        if ((state.IsExplicitVR && state.CurrentVr.Is32BitLength()) || !state.IsExplicitVR)
        {
            if (state.IsExplicitVR)
            {
                if (!buffer.TryReadExplicitVrLongValueLength(ref state.Position, out state.IntHolder))
                    return false;
            }
            else
            {
                if (!buffer.TryReadImplicitVrLongValueLength(ref state.Position, out state.IntHolder))
                    return false;
            }

            uint rawLongValueLength = (uint)state.IntHolder;

            if (state.CurrentVr == DicomVR.SQ)
            {
                if (state.CurrentSequence is { } currentSequence)
                    state.CurrentSequences.Push(currentSequence);

                if (state.CurrentSequenceItem is { } currentSequenceItem)
                    state.CurrentSequenceItems.Push(currentSequenceItem);

                long? sequenceLength = rawLongValueLength == UndefinedLength ? null : rawLongValueLength;
                long? sequenceEndPosition = state.Position + sequenceLength;
                state.CurrentSequence = new SequencePosition(sequenceEndPosition);
                state.CurrentSequenceItem = null;
                state.Handler.OnSequenceStart(state.CurrentGroupNumber, state.CurrentElementNumber);
                state.ParseStage = DicomParseStage.ParseGroup;
                return true;
            }

            if (rawLongValueLength == UndefinedLength)
            {
                state.IsParsingFragments = true;
                state.Handler.OnFragmentsStart(state.CurrentGroupNumber, state.CurrentElementNumber, state.CurrentVr);
                state.ParseStage = DicomParseStage.ParseGroup;
                return true;
            }

            if (rawLongValueLength > MaxArrayLength)
                throw new NotSupportedException(
                    "DcmParser does not support DICOM files with values larger than 2 GB yet");

            state.LongValueLength = (int)rawLongValueLength;
            state.ShortValueLength = null;
        }
        else
        {
            if (!buffer.TryReadShort(ref state.Position, out state.ShortHolder))
                return false;

            state.ShortValueLength = (ushort)state.ShortHolder;
            state.LongValueLength = null;
        }

        state.ParseStage = DicomParseStage.ParseValue;
        return true;
    }

    static bool TryParseValue(ref DicomByteBuffer buffer, ref DicomParseState state)
    {
        if (state.ShortValueLength is not null)
        {
            ushort remaining = (ushort)buffer.Remaining;

            if (state.CurrentValueMemory is null)
            {
                if (remaining >= state.ShortValueLength.Value)
                {
                    state.CurrentValueMemory = AllocateShortMemory(ref state, state.ShortValueLength.Value);
                    state.CurrentShortValueMemoryOffset = 0;
                    buffer.TryRead(ref state.Position, state.CurrentValueMemory.Value.Span);
                }
                else
                {
                    state.CurrentValueMemory = AllocateShortMemory(ref state, state.ShortValueLength.Value);
                    state.CurrentShortValueMemoryOffset = remaining;
                    Span<byte> currentValueSpan = state.CurrentValueMemory.Value.Span[..remaining];
                    buffer.TryRead(ref state.Position, currentValueSpan);
                    return false;
                }
            }
            else
            {
                int bytesToRead = Math.Min(
                    state.ShortValueLength.Value - state.CurrentShortValueMemoryOffset,
                    remaining);
                Span<byte> currentValueSpan =
                    state.CurrentValueMemory.Value.Span.Slice(state.CurrentShortValueMemoryOffset, bytesToRead);
                buffer.TryRead(ref state.Position, currentValueSpan);
                state.CurrentShortValueMemoryOffset += (ushort)bytesToRead;

                if (state.CurrentShortValueMemoryOffset < state.ShortValueLength.Value)
                    return false;
            }
        }
        else if (state.LongValueLength is not null)
        {
            int remaining = (int)buffer.Remaining;

            if (state.CurrentValueMemory is null)
            {
                if (remaining >= state.LongValueLength.Value)
                {
                    state.CurrentValueMemory = AllocateLongMemory(ref state, state.LongValueLength.Value);
                    Span<byte> currentValueSpan = state.CurrentValueMemory.Value.Span;
                    buffer.TryRead(ref state.Position, currentValueSpan);
                }
                else
                {
                    state.CurrentValueMemory = AllocateLongMemory(ref state, state.LongValueLength.Value);
                    state.CurrentLongValueMemoryOffset = remaining;
                    Span<byte> currentValueSpan = state.CurrentValueMemory.Value.Span[..remaining];
                    buffer.TryRead(ref state.Position, currentValueSpan);
                    return false;
                }
            }
            else
            {
                int bytesToRead = Math.Min(state.LongValueLength.Value - state.CurrentLongValueMemoryOffset,
                    remaining);
                Span<byte> currentValueSpan = state.CurrentValueMemory.Value.Span.Slice(
                    state.CurrentLongValueMemoryOffset,
                    bytesToRead);
                buffer.TryRead(ref state.Position, currentValueSpan);
                state.CurrentLongValueMemoryOffset += bytesToRead;

                if (state.CurrentLongValueMemoryOffset < state.LongValueLength.Value)
                    return false;
            }
        }
        else
        {
            throw new DicomException(
                $"Both {nameof(state.ShortValueLength)} and {nameof(state.LongValueLength)} are null, this is a bug");
        }

        if (state.IsParsingFragments)
        {
            state.Handler.OnFragment(state.CurrentValueMemory.Value);
            state.CurrentValueMemory = null;
            state.CurrentShortValueMemoryOffset = 0;
            state.CurrentLongValueMemoryOffset = 0;
        }
        else
        {
            var valueMemory = state.CurrentValueMemory.Value;
            state.CurrentValueMemory = null;
            state.CurrentShortValueMemoryOffset = 0;
            state.CurrentLongValueMemoryOffset = 0;

            if (state.CurrentElementNumber != GroupLengthElement)
            {
                state.Handler.OnItem(state.CurrentGroupNumber, state.CurrentElementNumber,
                    state.CurrentVr, valueMemory);
            }

            if (state.CurrentGroupNumber == DicomTags.TransferSyntaxUID.Group
                && state.CurrentElementNumber == DicomTags.TransferSyntaxUID.Element
                && valueMemory.Span.SequenceEqual(_implicitVRLittleEndianBytes))
            {
                state.SetToImplicitVrAfterFileMetaInfo = true;
            }

            if (state.CurrentGroupNumber == DicomTags.SpecificCharacterSet.Group
                && state.CurrentElementNumber == DicomTags.SpecificCharacterSet.Element
                && state.ValueParser.CS.TryParse(valueMemory.Span, out string? specificCharacterSet)
                && DicomEncoding.TryParse(specificCharacterSet, out Encoding? encoding))
            {
                state.Handler.OnEncoding(encoding);
            }
        }

        state.ParseStage = DicomParseStage.ParseGroup;
        return true;
    }

    static Memory<byte> AllocateShortMemory(ref DicomParseState state, ushort memoryLength)
    {
        var memory = state.Memory;
        if (memory is not null && memory.Value.Length - state.MemoryOffset >= memoryLength)
        {
            int oldMemoryOffset = state.MemoryOffset;
            state.MemoryOffset += memoryLength;
            return memory.Value.Memory.Slice(oldMemoryOffset, memoryLength);
        }

        memory = new DicomMemory(_shortArrayPool, MinimumMemoryBlockSize);
        state.MemoryOffset = memoryLength;
        state.Memories.Add(memory.Value);
        return memory.Value.Memory[..memoryLength];
    }

    static Memory<byte> AllocateLongMemory(ref DicomParseState state, int memoryLength)
    {
        if (state.Memory is { } shortMemory && shortMemory.Length - state.MemoryOffset >= memoryLength)
        {
            int oldMemoryOffset = state.MemoryOffset;
            state.MemoryOffset += memoryLength;
            return shortMemory.Memory.Slice(oldMemoryOffset, memoryLength);
        }

        var pool = memoryLength < 1_048_576 ? _shortArrayPool : _longArrayPool;
        var memory = new DicomMemory(pool, memoryLength);
        state.Memories.Add(memory);
        return memory.Memory;
    }
}
