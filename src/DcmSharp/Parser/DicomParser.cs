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
    Task<DicomDataset> ParseAsync(FileInfo file, CancellationToken cancellationToken = default);
}

internal sealed class DicomParser : IDicomParser
{
    private static readonly ArrayPool<byte> _shortArrayPool = ArrayPool<byte>.Shared;
    private static readonly ArrayPool<byte> _longArrayPool = ArrayPool<byte>.Create(25 * 1024 * 1024, 32);

    private static readonly DicomItemDictionaryPool _largeDicomItemDictionaryPool =
        new DicomItemDictionaryPool(maxPoolSize: 64);

    private static readonly DicomItemDictionaryPool _smallDicomItemDictionaryPool =
        new DicomItemDictionaryPool(maxPoolSize: 256);

    private static readonly DicomDatasetsPool _datasetsPool = new DicomDatasetsPool(256, 8);
    private static readonly DicomMemoriesPool _memoriesPool = new DicomMemoriesPool(1024, 32);
    private static readonly DicomFragmentsPool _fragmentsPool = new DicomFragmentsPool(1024, 32);

    private static readonly FileStreamOptions _fileStreamOptions = new FileStreamOptions
    {
        Access = FileAccess.Read, Options = FileOptions.SequentialScan, Share = FileShare.Read, Mode = FileMode.Open
    };

    private readonly ILogger<DicomParser> _logger;
    private readonly DicomValueParser _valueParser;

    /// <summary>
    /// This is the block size when data needs to buffered
    /// </summary>
    private const int BufferSize = 1024 * 1024; // 1 MB

    /// <summary>
    /// This the minimum size of each memory block that will hold the data of the DICOM dataset
    /// </summary>
    private const int MinimumMemoryBlockSize = 16384;

    /// <summary>
    /// This is the maximum number of bytes C# allows to store in a byte array (note that it is slightly lower than int.MaxValue)
    /// </summary>
    private const int MaxArrayLength = 2_147_483_591;

    /// <summary>
    /// DICOM files typically use this length for sequences
    /// </summary>
    private const uint UndefinedLength = 0xffffffff;

    /// <summary>
    /// The File Meta Information Group number
    /// </summary>
    private const ushort FileMetaInformationGroup = 0x0002;

    /// <summary>
    /// The Group Length element number
    /// </summary>
    private const ushort GroupLengthElement = 0x0000;

    private static readonly byte[] _implicitVRLittleEndianBytes = "1.2.840.10008.1.2"u8.ToArray() switch
    {
        { } even when even.Length % 2 == 0 => even,
        { } uneven => [..uneven, 0],
        _ => throw new UnreachableException()
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

    [SuppressMessage("Usage", "MA0004:Use Task.ConfigureAwait")]
    public async Task<DicomDataset> ParseAsync(FileInfo file, CancellationToken cancellationToken = default)
    {
        var pipe = new Pipe();
        var fillPipeTask = FillPipeAsync(file, pipe.Writer, cancellationToken);
        var readPipeTask = ReadPipeAsync(pipe.Reader, cancellationToken);
        await Task.WhenAll(fillPipeTask, readPipeTask);
        return await readPipeTask;
    }

    private async Task FillPipeAsync(FileInfo file, PipeWriter pipeWriter,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Open the file stream for reading
            await using var fileStream = file.Open(_fileStreamOptions);
            long fileLength = fileStream.Length;
            int bufferSize = (int)Math.Min(BufferSize, fileLength);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Request a memory block from the PipeWriter
                Memory<byte> memory = pipeWriter.GetMemory(bufferSize);

                // Read data from the file stream into the memory block
                int bytesRead = fileStream.Read(memory.Span);

                if (bytesRead == 0)
                {
                    break; // End of file reached
                }

                // Tell the PipeWriter how much data was actually read
                pipeWriter.Advance(bytesRead);

                // Make the data available to the PipeReader
                FlushResult result = await pipeWriter.FlushAsync(cancellationToken);

                if (result.IsCompleted || result.IsCanceled)
                {
                    break; // PipeReader signaled completion or cancellation
                }
            }
        }
        finally
        {
            // Signal to the PipeWriter that we're done writing
            await pipeWriter.CompleteAsync();
        }
    }

    private async Task<DicomDataset> ReadPipeAsync(PipeReader reader, CancellationToken cancellationToken = default)
    {
        var dicomDataset =
            new DicomDataset(_largeDicomItemDictionaryPool, new DicomMemories(_memoriesPool), _valueParser);
        long position;

        // Ensure DICOM3
        // The first 128 bytes should be empty
        // Then, the next 4 bytes should spell DICM
        ReadResult result = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        if (result.Buffer.Length < 132)
        {
            throw new DicomException("DICOM file is too small to be valid, expected at least 132 bytes");
        }

        EnsureValidPreamble(result.Buffer.Slice(128, 4));
        reader.AdvanceTo(result.Buffer.GetPosition(132));
        position = 132;

        var state = new DicomParseState
        {
            DicomDataset = dicomDataset, Logger = _logger, ValueParser = _valueParser, Position = position,
        };

        try
        {
            while (true)
            {
                result = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);

                if (result.IsCompleted)
                {
                    break;
                }

                ReadOnlySequence<byte> sequence = result.Buffer;
                DicomByteBuffer buffer = new DicomByteBuffer(result.Buffer);
                long originalPosition = state.Position;
                Parse(ref buffer, ref state, cancellationToken);
                long parsed = state.Position - originalPosition;
                if (parsed == 0)
                {
                    if (result.IsCompleted)
                    {
                        throw new DicomException(
                            $"DICOM file could not be parsed completely, parsing stopped at position {position}");
                    }
                }
                else
                {
                    position += parsed;
                    sequence = sequence.Slice(parsed);
                }

                reader.AdvanceTo(sequence.Start, sequence.End);
                if (result.IsCompleted)
                {
                    break;
                }
            }

            return state.DicomDataset;
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
            {
                throw new DicomException(
                    $"This file does not look like a DICOM file, expected 'DICM' at position 128-132");
            }

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
                $"This file does not look like a DICOM file, expected 'DICM' at position 128-132");
        }
    }

    static void Parse(ref DicomByteBuffer buffer, ref DicomParseState state, CancellationToken cancellationToken)
    {
        while (!buffer.IsEmpty)
        {
            cancellationToken.ThrowIfCancellationRequested();

            switch (state.ParseStage)
            {
                case DicomParseStage.ParseGroup:
                    if (!TryParseGroup(ref buffer, ref state))
                    {
                        return;
                    }

                    // ParseGroup always transitions to ParseElement
                    goto case DicomParseStage.ParseElement;
                case DicomParseStage.ParseElement:
                    if (!TryParseElement(ref buffer, ref state))
                    {
                        return;
                    }

                    // Implicit VR files or certain items skip the VR parsing stage
                    if (state.ParseStage == DicomParseStage.ParseLength)
                    {
                        goto case DicomParseStage.ParseLength;
                    }

                    goto case DicomParseStage.ParseVR;
                case DicomParseStage.ParseVR:
                    if (!TryParseVr(ref buffer, ref state))
                    {
                        return;
                    }

                    // ParseVR always transitions to ParseLength
                    goto case DicomParseStage.ParseLength;
                case DicomParseStage.ParseLength:
                    if (!TryParseLength(ref buffer, ref state))
                    {
                        return;
                    }

                    // Sequence related logic may cause a jump from ParseLength to ParseGroup again
                    if (state.ParseStage == DicomParseStage.ParseGroup)
                    {
                        goto case DicomParseStage.ParseGroup;
                    }

                    goto case DicomParseStage.ParseValue;
                case DicomParseStage.ParseValue:
                    if (!TryParseValue(ref buffer, ref state))
                    {
                        return;
                    }

                    break;
                default:
                    throw new DicomException("Unknown parse stage: " + state.ParseStage);
            }

            if (state.CurrentSequence is { } currentSequence)
            {
                // Handle sequence item with explicit length instead of item delimitation items
                if (state.CurrentSequenceItem is { } currentSequenceItem
                    && currentSequenceItem.EndPosition is { } sequenceItemEndPosition
                    && state.Position >= sequenceItemEndPosition)
                {
                    CloseCurrentSequenceItem(ref state, currentSequence, currentSequenceItem);
                    state.ParseStage = DicomParseStage.ParseGroup;
                }

                // Handle sequence with explicit length instead of sequence delimitation items
                if (currentSequence.EndPosition is { } currentSequenceEndPosition
                    && state.Position >= currentSequenceEndPosition)
                {
                    CloseCurrentSequence(ref state, currentSequence);
                    state.ParseStage = DicomParseStage.ParseGroup;
                }
            }
        }
    }

    static bool TryParseGroup(ref DicomByteBuffer buffer, ref DicomParseState state)
    {
        if (!buffer.TryReadShort(ref state.Position, out state.ShortHolder))
        {
            // state.Logger.LogTrace("Not enough bytes to parse group, returning");
            return false;
        }

        state.CurrentGroupNumber = (ushort)state.ShortHolder;
        state.ParseStage = DicomParseStage.ParseElement;
        return true;
    }

    static bool TryParseElement(ref DicomByteBuffer buffer, ref DicomParseState state)
    {
        if (!buffer.TryReadShort(ref state.Position, out state.ShortHolder))
        {
            // state.Logger.LogTrace("Not enough bytes to parse element, returning");
            return false;
        }

        state.CurrentElementNumber = (ushort)state.ShortHolder;
        // state.Logger.LogTrace("Parsed element 0x{ElementNumber:x4}", state.CurrentElementNumber);

        if (state.CurrentGroupNumber > FileMetaInformationGroup)
        {
            // When the file meta info group is finished, and the transfer syntax is implicit VR,
            // we need to switch to implicit VR from here on out
            if (state is { IsExplicitVR: true, SetToImplicitVrAfterFileMetaInfo: true })
            {
                state.IsExplicitVR = false;
            }

            // Items, Item Delimitation Items and Sequence Delimitation Items do not provide a VR at all
            if (state.CurrentGroupNumber is ItemGroup &&
                state.CurrentElementNumber is Item or ItemDelimitationItem or SequenceDelimitationItem)
            {
                state.ParseStage = DicomParseStage.ParseLength;
                return true;
            }

            // Implicit VR files do not provide a VR, the VR must be inferred from the group and element numbers
            if (!state.IsExplicitVR)
            {
                state.CurrentVr = DicomTagsIndex.TryLookup(state.CurrentGroupNumber, state.CurrentElementNumber,
                    out var dicomTag)
                    ? dicomTag.VR
                    : DicomVR.UN;

                if (state is { CurrentVr: DicomVR.UN, CurrentElementNumber: GroupLengthElement })
                {
                    // Group Length = UL
                    state.CurrentVr = DicomVR.UL;
                }

                state.ParseStage = DicomParseStage.ParseLength;
                return true;
            }
        }

        state.ParseStage = DicomParseStage.ParseVR;
        return true;
    }

    static bool TryParseVr(ref DicomByteBuffer buffer, ref DicomParseState state)
    {
        // VR uses 2 bytes, just like a short
        if (!buffer.TryReadVr(ref state.Position, out byte b1, out byte b2))
        {
            // state.Logger.LogTrace("Not enough bytes to parse VR, returning");
            return false;
        }

        if (!DicomVRParser.TryParse(b1, b2, out DicomVR? parsedVr))
        {
            throw new DicomException(
                $"Invalid DICOM file, could not parse ({b1:x4},{b2:x4}) to a known DICOM VR");
        }

        // state.Logger.LogTrace("Parsed VR {VR}", parsedVr.Value);
        state.CurrentVr = parsedVr.Value;
        state.ParseStage = DicomParseStage.ParseLength;
        return true;
    }

    static bool TryParseLength(ref DicomByteBuffer buffer, ref DicomParseState state)
    {
        // Sequence Items or Sequence Delimitation Items use 4 bytes for their value length
        // When the length is specified, DICOM files are not obliged to add an Item Delimitation Item at the end of each item
        if (state.CurrentGroupNumber is ItemGroup)
        {
            if (!buffer.TryReadInt(ref state.Position, out state.IntHolder))
            {
                // state.Logger.LogTrace("Not enough bytes to parse element, returning");
                return false;
            }

            uint rawLongValueLength = (uint)state.IntHolder;

            // Handle sequences
            if (state.CurrentSequence is { } currentSequence)
            {
                // Handle sequence item
                if (state.CurrentElementNumber is Item)
                {
                    if (state.CurrentSequenceItem is not null)
                    {
                        throw new DicomException(
                            $"Encountered a DICOM sequence item ({state.CurrentGroupNumber:x4},{state.CurrentElementNumber:x4}) without a preceding DICOM delimitation item");
                    }

                    // Open a new sequence item

                    // Sequence items may use explicit lengths instead of item delimitation items
                    // In that case we must close the sequence item after the specified number of bytes have been parsed
                    long? sequenceItemLength = rawLongValueLength == UndefinedLength ? null : rawLongValueLength;
                    long? sequenceItemEndPosition = state.Position + sequenceItemLength;
                    DicomDataset sequenceItemDataset = new DicomDataset(_smallDicomItemDictionaryPool,
                        new DicomMemories(_memoriesPool), state.ValueParser);
                    state.CurrentSequenceItem = new DicomSequenceItem(sequenceItemDataset, sequenceItemEndPosition);

                    state.ParseStage = DicomParseStage.ParseGroup;
                    return true;
                }

                // Handle item delimitation item
                if (state.CurrentElementNumber is ItemDelimitationItem)
                {
                    if (state.CurrentSequenceItem is not { } currentSequenceItem)
                    {
                        throw new DicomException(
                            "Encountered a DICOM item delimitation item without a preceding DICOM item");
                    }

                    CloseCurrentSequenceItem(ref state, currentSequence, currentSequenceItem);
                    state.ParseStage = DicomParseStage.ParseGroup;
                    return true;
                }

                // Handle sequence delimitation item
                if (state.CurrentElementNumber is SequenceDelimitationItem)
                {
                    CloseCurrentSequence(ref state, currentSequence);
                    state.ParseStage = DicomParseStage.ParseGroup;
                    return true;
                }
            }

            // Handle fragments
            else if (state.CurrentFragments is not null)
            {
                if (state.CurrentElementNumber is Item)
                {
                    if (rawLongValueLength > MaxArrayLength)
                    {
                        throw new NotSupportedException(
                            "DcmParser does not support DICOM files with values larger than 2 GB yet");
                    }

                    state.LongValueLength = (int)rawLongValueLength;
                    // state.Logger.LogTrace("Parsed Value Length {LongValueLength}", state.LongValueLength);
                    state.ShortValueLength = null;

                    state.ParseStage = DicomParseStage.ParseValue;
                    return true;
                }

                if (state.CurrentElementNumber is SequenceDelimitationItem)
                {
                    state.CurrentDicomItem = new DicomItem(state.CurrentFragmentsGroupNumber,
                        state.CurrentFragmentsElementNumber, state.CurrentFragmentsVR,
                        DicomItemContent.Create(state.CurrentFragments.ToReadOnly()));
                    // state.Logger.LogTrace("Parsed fragmented tag {Tag}", state.CurrentDicomItem);
                    state.DicomDataset.Add(state.CurrentFragmentsGroupNumber,
                        state.CurrentFragmentsElementNumber,
                        state.CurrentDicomItem.Value);
                    state.CurrentFragmentsGroupNumber = default;
                    state.CurrentFragmentsElementNumber = default;
                    state.CurrentFragmentsVR = default;
                    state.CurrentFragments = null;

                    state.ParseStage = DicomParseStage.ParseGroup;
                    return true;
                }

                throw new DicomException(
                    "Expected one of Item or SequenceDelimitationItem while parsing fragment sequence");
            }
        }

        // Implicit VR always uses 32 bit length
        // Explicit VR sometimes uses 32 bit length, sometimes 16 bit length, depending on the VR
        // However, reading the value length for Explicit VR requires skipping 2 bytes, while implicit VR does not
        if ((state.IsExplicitVR && state.CurrentVr.Is32BitLength()) || !state.IsExplicitVR)
        {
            if (state.IsExplicitVR)
            {
                if (!buffer.TryReadExplicitVrLongValueLength(ref state.Position, out state.IntHolder))
                {
                    // state.Logger.LogTrace("Not enough bytes to parse explicit VR value length, returning");
                    return false;
                }
            }
            else
            {
                if (!buffer.TryReadImplicitVrLongValueLength(ref state.Position, out state.IntHolder))
                {
                    // state.Logger.LogTrace("Not enough bytes to parse implicit VR value length, returning");
                    return false;
                }
            }

            uint rawLongValueLength = (uint)state.IntHolder;

            if (state.CurrentVr == DicomVR.SQ)
            {
                // New sequence started, store current level in a stack
                if (state.CurrentSequence is { } currentSequence)
                {
                    state.CurrentSequences.Push(currentSequence);
                }

                if (state.CurrentSequenceItem is { } currentSequenceItem)
                {
                    state.CurrentSequenceItems.Push(currentSequenceItem);
                }

                // Sequences may use explicit lengths instead of sequence delimitation items
                // In that case we must close the sequence after the specified number of bytes have been parsed
                long? sequenceLength = rawLongValueLength == UndefinedLength ? null : rawLongValueLength;
                long? sequenceEndPosition = state.Position + sequenceLength;
                var dicomSequence = new DicomSequence(state.CurrentGroupNumber, state.CurrentElementNumber,
                    new DicomDatasets(_datasetsPool), sequenceEndPosition);
                state.CurrentSequence = dicomSequence;
                state.CurrentSequenceItem = null;
                state.ParseStage = DicomParseStage.ParseGroup;
                return true;
            }

            if (rawLongValueLength == UndefinedLength)
            {
                state.CurrentFragmentsGroupNumber = state.CurrentGroupNumber;
                state.CurrentFragmentsElementNumber = state.CurrentElementNumber;
                state.CurrentFragmentsVR = state.CurrentVr;
                state.CurrentFragments = new DicomFragments(_fragmentsPool);
                state.ParseStage = DicomParseStage.ParseGroup;
                return true;
            }

            if (rawLongValueLength > MaxArrayLength)
            {
                throw new NotSupportedException(
                    "DcmParser does not support DICOM files with values larger than 2 GB yet");
            }

            state.LongValueLength = (int)rawLongValueLength;

            // state.Logger.LogTrace("Parsed Value Length {LongValueLength}", state.LongValueLength);
            state.ShortValueLength = null;
        }
        else
        {
            if (!buffer.TryReadShort(ref state.Position, out state.ShortHolder))
            {
                // state.Logger.LogTrace("Not enough bytes to parse value length, returning");
                return false;
            }

            state.ShortValueLength = (ushort)state.ShortHolder;
            // state.Logger.LogTrace("Parsed Value Length {ShortValueLength}", state.ShortValueLength);
            state.LongValueLength = null;
        }

        state.ParseStage = DicomParseStage.ParseValue;
        return true;
    }

    static bool TryParseValue(ref DicomByteBuffer buffer, ref DicomParseState state)
    {
        // 16 bit length flow
        if (state.ShortValueLength is not null)
        {
            ushort remaining = (ushort)buffer.Remaining;

            // no memory allocated yet for this value
            if (state.CurrentValueMemory is null)
            {
                // We can parse the value in one go
                if (remaining > state.ShortValueLength.Value)
                {
                    state.CurrentValueMemory = AllocateShortMemory(ref state, state.ShortValueLength.Value);
                    state.CurrentShortValueMemoryOffset = 0;

                    buffer.TryRead(ref state.Position, state.CurrentValueMemory.Value.Span);
                }
                // We cannot parse the value in one go, prep memory and keep track of how much we have left to parse
                else
                {
                    state.CurrentValueMemory = AllocateShortMemory(ref state, state.ShortValueLength.Value);
                    state.CurrentShortValueMemoryOffset = remaining;
                    Span<byte> currentValueSpan = state.CurrentValueMemory.Value.Span[..remaining];
                    buffer.TryRead(ref state.Position, currentValueSpan);
                    // We still have more bytes to read
                    return false;
                }
            }
            // some memory was already allocated for this value
            else
            {
                // We are parsing the value in bits and pieces
                int bytesToRead = Math.Min(
                    state.ShortValueLength.Value - state.CurrentShortValueMemoryOffset,
                    remaining);

                Span<byte> currentValueSpan =
                    state.CurrentValueMemory.Value.Span.Slice(state.CurrentShortValueMemoryOffset, bytesToRead);
                buffer.TryRead(ref state.Position, currentValueSpan);
                state.CurrentShortValueMemoryOffset += (ushort)bytesToRead;

                if (state.CurrentShortValueMemoryOffset < state.ShortValueLength.Value)
                {
                    // We still have more bytes to read
                    return false;
                }
            }
        }
        // 32 bit length flow
        else if (state.LongValueLength is not null)
        {
            int remaining = (int)buffer.Remaining;

            // no memory allocated yet for this value
            if (state.CurrentValueMemory is null)
            {
                // We can parse the value in one go
                if (remaining > state.LongValueLength.Value)
                {
                    state.CurrentValueMemory = AllocateLongMemory(ref state, state.LongValueLength.Value);
                    Span<byte> currentValueSpan = state.CurrentValueMemory.Value.Span;
                    buffer.TryRead(ref state.Position, currentValueSpan);
                }
                // We cannot parse the value in one go, prep memory and keep track of how much we have left to parse
                else
                {
                    state.CurrentValueMemory = AllocateLongMemory(ref state, state.LongValueLength.Value);
                    state.CurrentLongValueMemoryOffset = remaining;
                    Span<byte> currentValueSpan = state.CurrentValueMemory.Value.Span[..remaining];
                    buffer.TryRead(ref state.Position, currentValueSpan);
                    // We still have more bytes to read
                    return false;
                }
            }
            // some memory was already allocated for this value
            else
            {
                // We are parsing the value in bits and pieces
                int bytesToRead = Math.Min(state.LongValueLength.Value - state.CurrentLongValueMemoryOffset,
                    remaining);
                Span<byte> currentValueSpan = state.CurrentValueMemory.Value.Span.Slice(
                    state.CurrentLongValueMemoryOffset,
                    bytesToRead);
                buffer.TryRead(ref state.Position, currentValueSpan);
                state.CurrentLongValueMemoryOffset += bytesToRead;

                if (state.CurrentLongValueMemoryOffset < state.LongValueLength.Value)
                {
                    // We still have more bytes to read
                    return false;
                }
            }
        }
        else
        {
            throw new DicomException(
                $"Both {nameof(state.ShortValueLength)} and {nameof(state.LongValueLength)} are null, this is a bug");
        }

        if (state.CurrentFragments is not null)
        {
            state.CurrentFragments.Add(state.CurrentValueMemory.Value);
            state.CurrentValueMemory = null;
            state.CurrentShortValueMemoryOffset = 0;
            state.CurrentLongValueMemoryOffset = 0;
        }
        else
        {
            state.CurrentDicomItem = new DicomItem(state.CurrentGroupNumber, state.CurrentElementNumber,
                state.CurrentVr, DicomItemContent.Create(state.CurrentValueMemory.Value));
            state.CurrentValueMemory = null;
            state.CurrentShortValueMemoryOffset = 0;
            state.CurrentLongValueMemoryOffset = 0;

            // If we're currently parsing a SQ item, add the newly parsed DICOM item to the sequence item dataset
            // instead of the top level dataset
            var dataset = state.CurrentSequenceItem?.DicomDataset ?? state.DicomDataset;

            // state.Logger.LogTrace("Parsed tag {Tag}", state.CurrentDicomItem);
            if (state.CurrentElementNumber != GroupLengthElement)
            {
                dataset.Add(state.CurrentGroupNumber, state.CurrentElementNumber,
                    state.CurrentDicomItem.Value);
            }

            if (state.CurrentGroupNumber == DicomTags.TransferSyntaxUID.Group
                && state.CurrentElementNumber == DicomTags.TransferSyntaxUID.Element
                && state.CurrentDicomItem.Value.Content.Value!.Value.Span.SequenceEqual(
                    _implicitVRLittleEndianBytes))
            {
                state.SetToImplicitVrAfterFileMetaInfo = true;
            }

            if (state.CurrentGroupNumber == DicomTags.SpecificCharacterSet.Group
                && state.CurrentElementNumber == DicomTags.SpecificCharacterSet.Element
                && state.CurrentDicomItem.Value.Content.Value is { } specificCharacterSetBytes
                && state.ValueParser.CS.TryParse(specificCharacterSetBytes.Span, out string? specificCharacterSet)
                && DicomEncoding.TryParse(specificCharacterSet, out Encoding? encoding))
            {
                dataset.Encoding = encoding;
            }
        }

        state.ParseStage = DicomParseStage.ParseGroup;
        return true;
    }

    static void CloseCurrentSequenceItem(ref DicomParseState state, DicomSequence currentSequence,
        DicomSequenceItem currentSequenceItem)
    {
        currentSequence.Items.Add(currentSequenceItem.DicomDataset);
        state.CurrentSequenceItem = null;
    }

    static void CloseCurrentSequence(ref DicomParseState state, DicomSequence currentSequence)
    {
        state.CurrentDicomItem = new DicomItem(currentSequence.Group,
            currentSequence.Element, DicomVR.SQ,
            DicomItemContent.Create(currentSequence.Items.ToReadOnly()));

        // state.Logger.LogTrace("Parsed sequence tag {Tag}", state.CurrentDicomItem);

        if (state.CurrentSequenceItems.TryPop(out var currentSequenceItem))
        {
            currentSequenceItem.DicomDataset.Add(currentSequence.Group, currentSequence.Element,
                state.CurrentDicomItem.Value);
            state.CurrentSequenceItem = currentSequenceItem;
        }
        else
        {
            state.DicomDataset.Add(currentSequence.Group, currentSequence.Element, state.CurrentDicomItem.Value);
            state.CurrentSequenceItem = null;
        }

        if (state.CurrentSequences.TryPop(out currentSequence))
        {
            state.CurrentSequence = currentSequence;
        }
        else
        {
            state.CurrentSequence = null;
        }
    }

    static Memory<byte> AllocateShortMemory(ref DicomParseState state, ushort memoryLength)
    {
        var memory = state.Memory;
        // Enough space remains in current memory, just advance the offset
        if (memory is not null && memory.Value.Length - state.MemoryOffset >= memoryLength)
        {
            int oldMemoryOffset = state.MemoryOffset;
            state.MemoryOffset += memoryLength;
            return memory.Value.Memory.Slice(oldMemoryOffset, memoryLength);
        }

        // Not enough space remains, allocate new memory
        memory = new DicomMemory(_shortArrayPool, MinimumMemoryBlockSize);
        state.MemoryOffset = memoryLength;
        state.DicomDataset.ReleaseOnDispose(memory.Value);
        return memory.Value.Memory.Slice(0, memoryLength);
    }

    static Memory<byte> AllocateLongMemory(ref DicomParseState state, int memoryLength)
    {
        // Optimization: if we can still fit this into the short memory, use that instead
        if (state.Memory is { } shortMemory && shortMemory.Length - state.MemoryOffset >= memoryLength)
        {
            int oldMemoryOffset = state.MemoryOffset;
            state.MemoryOffset += memoryLength;
            return shortMemory.Memory.Slice(oldMemoryOffset, memoryLength);
        }

        var pool = memoryLength < 1_048_576 ? _shortArrayPool : _longArrayPool;
        var memory = new DicomMemory(pool, memoryLength);
        state.DicomDataset.ReleaseOnDispose(memory);
        return memory.Memory;
    }
}
