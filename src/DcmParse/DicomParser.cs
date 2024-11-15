// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace DcmParse;

public sealed class DicomParser
{
    private static readonly ArrayPool<byte> _shortArrayPool = ArrayPool<byte>.Shared;
    private static readonly ArrayPool<byte> _longArrayPool = ArrayPool<byte>.Create(25 * 1024 * 1024, 32);
    private static readonly DicomDictionaryPool _dicomFilePool = new DicomDictionaryPool(maxPoolSize: 64, 256);
    private static readonly DicomDictionaryPool _sequenceItemsPool = new DicomDictionaryPool(maxPoolSize: 256, 16);
    private static readonly DicomMemoriesPool _memoriesPool = new DicomMemoriesPool(1024, 32);

    private readonly ILogger<DicomParser> _logger;

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
    /// The Transfer Syntax element number
    /// </summary>
    private const ushort TransferSyntaxElement = 0x0010;

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

    public DicomParser(ILogger<DicomParser> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [SuppressMessage("Usage", "MA0004:Use Task.ConfigureAwait")]
    public async Task<DicomDataset> ParseAsync(FileInfo file, CancellationToken cancellationToken = default)
    {
        await using var fileStream = file.OpenRead();
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
            await using var fileStream = file.OpenRead();

            while (true)
            {
                // Request a memory block from the PipeWriter
                Memory<byte> memory = pipeWriter.GetMemory(BufferSize);

                // Read data from the file stream into the memory block
                int bytesRead = await fileStream.ReadAsync(memory, cancellationToken);

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
        var dicomDataset = new DicomDataset(_dicomFilePool, new DicomMemories(_memoriesPool));
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

        var state = new DicomParseState { DicomDataset = dicomDataset, Logger = _logger };

        byte[] vrBytes = ArrayPool<byte>.Shared.Rent(2);
        try
        {
            while (true)
            {
                result = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);

                if (result.IsCompleted)
                {
                    break;
                }

                var resultBuffer = result.Buffer;
                int parsed = Parse(ref state, ref resultBuffer, vrBytes, cancellationToken);
                if (parsed == 0)
                {
                    if (result.IsCompleted)
                    {
                        throw new DicomException($"DICOM file could not be parsed completely, parsing stopped at position {position}");
                    }
                }
                else
                {
                    position += parsed;
                    resultBuffer = resultBuffer.Slice(parsed);
                }
                reader.AdvanceTo(resultBuffer.Start, resultBuffer.End);
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
            ArrayPool<byte>.Shared.Return(vrBytes);
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

    static int Parse(ref DicomParseState state, ref ReadOnlySequence<byte> buffer, byte[] vrBytes, CancellationToken cancellationToken)
    {
        var vrSpan = vrBytes.AsSpan()[..2];
        int consumed = 0;
        var bufferSpan = buffer.IsSingleSegment ? buffer.FirstSpan : default;
        var bufferReader = bufferSpan.IsEmpty ? new SequenceReader<byte>(buffer) : default;

        while (!IsEmpty(ref bufferSpan, ref bufferReader))
        {
            cancellationToken.ThrowIfCancellationRequested();

            switch (state.ParseStage)
            {
                case DicomParseStage.ParseGroup:
                    if (!TryReadShort(ref consumed, ref bufferSpan, ref bufferReader, out state.ShortHolder))
                    {
                        state.Logger.LogTrace("Not enough bytes to parse group, returning");
                        return consumed;
                    }

                    state.CurrentGroupNumber = (ushort)state.ShortHolder;
                    state.Logger.LogTrace("Parsed group 0x{GroupNumber:x4}", state.CurrentGroupNumber);
                    state.ParseStage = DicomParseStage.ParseElement;
                    goto case DicomParseStage.ParseElement;
                case DicomParseStage.ParseElement:
                    if (!TryReadShort(ref consumed, ref bufferSpan, ref bufferReader, out state.ShortHolder))
                    {
                        state.Logger.LogTrace("Not enough bytes to parse element, returning");
                        return consumed;
                    }

                    state.CurrentElementNumber = (ushort)state.ShortHolder;
                    state.Logger.LogTrace("Parsed element 0x{ElementNumber:x4}", state.CurrentElementNumber);

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
                            goto case DicomParseStage.ParseLength;
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
                            goto case DicomParseStage.ParseLength;
                        }
                    }

                    state.ParseStage = DicomParseStage.ParseVR;
                    goto case DicomParseStage.ParseVR;
                case DicomParseStage.ParseVR:
                    if (!TryRead(ref consumed, ref bufferSpan, ref bufferReader, vrSpan))
                    {
                        state.Logger.LogTrace("Not enough bytes to parse VR, returning");
                        return consumed;
                    }

                    if (!DicomVRParser.TryParse(vrSpan, out DicomVR? parsedVr))
                    {
                        throw new DicomException(
                            $"Invalid DICOM file, could not parse {string.Join(", ", vrSpan.ToArray().Select(x => $"{x:x4}"))} to a known DICOM VR");
                    }

                    state.CurrentVr = parsedVr.Value;
                    state.Logger.LogTrace("Parsed VR {VR}", state.CurrentVr);
                    goto case DicomParseStage.ParseLength;
                case DicomParseStage.ParseLength:

                    // Sequence Items or Sequence Delimitation Items use 4 bytes for their value length
                    // When the length is specified, DICOM files are not obliged to add an Item Delimitation Item at the end of each item
                    if (state.CurrentGroupNumber is ItemGroup)
                    {
                        if (!TryReadInt(ref consumed, ref bufferSpan, ref bufferReader, out state.IntHolder))
                        {
                            state.Logger.LogTrace("Not enough bytes to parse element, returning");
                            return consumed;
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
                                    throw new DicomException($"Encountered a DICOM sequence item ({state.CurrentGroupNumber:x4},{state.CurrentElementNumber:x4}) without a preceding DICOM delimitation item");
                                }

                                if (rawLongValueLength != UndefinedLength)
                                {
                                    throw new DicomException($"Encountered a DICOM sequence item ({state.CurrentGroupNumber:x4},{state.CurrentElementNumber:x4}) with explicit length ({rawLongValueLength}) but that is not supported yet");
                                }

                                // Open a new sequence item
                                state.CurrentSequenceItem = new DicomDataset(_sequenceItemsPool, new DicomMemories(_memoriesPool));
                                state.ParseStage = DicomParseStage.ParseGroup;
                                goto case DicomParseStage.ParseGroup;
                            }

                            // Handle item delimitation item
                            if (state.CurrentElementNumber is ItemDelimitationItem)
                            {
                                if (state.CurrentSequenceItem is null)
                                {
                                    throw new DicomException(
                                        "Encountered a DICOM item delimitation item without a preceding DICOM item");
                                }

                                currentSequence.Items.Add(state.CurrentSequenceItem.Value);
                                state.CurrentSequenceItem = null;
                                state.ParseStage = DicomParseStage.ParseGroup;
                                goto case DicomParseStage.ParseGroup;
                            }

                            // Handle sequence delimitation item
                            if (state.CurrentElementNumber is SequenceDelimitationItem)
                            {
                                state.CurrentDicomItem = new DicomItem(currentSequence.Group,
                                    currentSequence.Element, DicomVR.SQ, DicomItemContent.Create(currentSequence.Items));
                                state.Logger.LogTrace("Parsed sequence tag {Tag}", state.CurrentDicomItem);
                                state.DicomDataset.Add(currentSequence.Group, currentSequence.Element,
                                    state.CurrentDicomItem.Value);
                                if (state.CurrentSequences.TryPop(out currentSequence))
                                {
                                    state.CurrentSequence = currentSequence;
                                }
                                else
                                {
                                    state.CurrentSequence = null;
                                }
                                state.CurrentSequenceItem = null;

                                state.ParseStage = DicomParseStage.ParseGroup;
                                goto case DicomParseStage.ParseGroup;
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
                                state.Logger.LogTrace("Parsed Value Length {LongValueLength}", state.LongValueLength);
                                state.ShortValueLength = null;

                                state.ParseStage = DicomParseStage.ParseValue;
                                goto case DicomParseStage.ParseValue;
                            }

                            if (state.CurrentElementNumber is SequenceDelimitationItem)
                            {
                                state.CurrentDicomItem = new DicomItem(state.CurrentFragmentsGroupNumber,
                                    state.CurrentFragmentsElementNumber, state.CurrentFragmentsVR,
                                    DicomItemContent.Create(state.CurrentFragments));
                                state.Logger.LogTrace("Parsed fragmented tag {Tag}", state.CurrentDicomItem);
                                state.DicomDataset.Add(state.CurrentFragmentsGroupNumber, state.CurrentFragmentsElementNumber,
                                    state.CurrentDicomItem.Value);
                                state.CurrentFragmentsGroupNumber = default;
                                state.CurrentFragmentsElementNumber = default;
                                state.CurrentFragmentsVR = default;
                                state.CurrentFragments = null;

                                state.ParseStage = DicomParseStage.ParseGroup;
                                goto case DicomParseStage.ParseGroup;
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
                            if (!TryReadExplicitVrLongValueLength(ref consumed, ref bufferSpan, ref bufferReader,
                                    out state.IntHolder))
                            {
                                state.Logger.LogTrace("Not enough bytes to parse explicit VR value length, returning");
                                return consumed;
                            }
                        }
                        else
                        {
                            if (!TryReadImplicitVrLongValueLength(ref consumed, ref bufferSpan, ref bufferReader,
                                    out state.IntHolder))
                            {
                                state.Logger.LogTrace("Not enough bytes to parse implicit VR value length, returning");
                                return consumed;
                            }
                        }

                        uint rawLongValueLength = (uint)state.IntHolder;

                        if (state.CurrentVr == DicomVR.SQ)
                        {
                            var dicomSequence = new DicomSequence(state.CurrentGroupNumber, state.CurrentElementNumber, []);
                            if (state.CurrentSequence is { } currentSequence)
                            {
                                state.CurrentSequences.Push(currentSequence);
                            }
                            state.CurrentSequence = dicomSequence;
                            state.ParseStage = DicomParseStage.ParseGroup;
                            goto case DicomParseStage.ParseGroup;
                        }

                        if (rawLongValueLength == UndefinedLength)
                        {
                            state.CurrentFragmentsGroupNumber = state.CurrentGroupNumber;
                            state.CurrentFragmentsElementNumber = state.CurrentElementNumber;
                            state.CurrentFragmentsVR = state.CurrentVr;
                            state.CurrentFragments = new List<Memory<byte>>(4);
                            state.ParseStage = DicomParseStage.ParseGroup;
                            goto case DicomParseStage.ParseGroup;
                        }

                        if (rawLongValueLength > MaxArrayLength)
                        {
                            throw new NotSupportedException(
                                "DcmParser does not support DICOM files with values larger than 2 GB yet");
                        }

                        state.LongValueLength = (int)rawLongValueLength;

                        state.Logger.LogTrace("Parsed Value Length {LongValueLength}", state.LongValueLength);
                        state.ShortValueLength = null;
                    }
                    else
                    {
                        if (!TryReadShortValueLength(ref consumed, ref bufferSpan, ref bufferReader,
                                out state.ShortHolder))
                        {
                            state.Logger.LogTrace("Not enough bytes to parse value length, returning");
                            return consumed;
                        }

                        state.ShortValueLength = (ushort)state.ShortHolder;
                        state.Logger.LogTrace("Parsed Value Length {ShortValueLength}", state.ShortValueLength);
                        state.LongValueLength = null;
                    }

                    state.ParseStage = DicomParseStage.ParseValue;
                    goto case DicomParseStage.ParseValue;
                case DicomParseStage.ParseValue:

                    // 16 bit length flow
                    if (state.ShortValueLength is not null)
                    {
                        ushort remaining = (ushort) GetRemaining(ref bufferSpan, ref bufferReader);

                        // no memory allocated yet for this value
                        if (state.CurrentValueMemory is null)
                        {
                            // We can parse the value in one go
                            if (remaining > state.ShortValueLength.Value)
                            {
                                state.CurrentValueMemory = AllocateShortMemory(ref state.Memory, ref state.MemoryOffset,
                                    ref state.DicomDataset, state.ShortValueLength.Value);
                                state.CurrentShortValueMemoryOffset = 0;

                                TryRead(ref consumed, ref bufferSpan, ref bufferReader,
                                    state.CurrentValueMemory.Value.Span);
                            }
                            // We cannot parse the value in one go, prep memory and keep track of how much we have left to parse
                            else
                            {
                                state.CurrentValueMemory = AllocateShortMemory(ref state.Memory, ref state.MemoryOffset,
                                    ref state.DicomDataset, state.ShortValueLength.Value);
                                state.CurrentShortValueMemoryOffset = remaining;
                                TryRead(ref consumed, ref bufferSpan, ref bufferReader,
                                    state.CurrentValueMemory.Value.Span.Slice(0, remaining));
                                // We still have more bytes to read
                                return consumed;
                            }
                        }
                        // some memory was already allocated for this value
                        else
                        {
                            // We are parsing the value in bits and pieces
                            int bytesToRead = Math.Min(state.ShortValueLength.Value - state.CurrentShortValueMemoryOffset,
                                remaining);
                            TryRead(ref consumed, ref bufferSpan, ref bufferReader,
                                state.CurrentValueMemory.Value.Span.Slice(state.CurrentShortValueMemoryOffset, bytesToRead));
                            state.CurrentShortValueMemoryOffset += (ushort)bytesToRead;

                            if (state.CurrentShortValueMemoryOffset < state.ShortValueLength.Value)
                            {
                                // We still have more bytes to read
                                return consumed;
                            }
                        }
                    }
                    // 32 bit length flow
                    else if (state.LongValueLength is not null)
                    {
                        int remaining = (int) GetRemaining(ref bufferSpan, ref bufferReader);

                        // no memory allocated yet for this value
                        if (state.CurrentValueMemory is null)
                        {
                            // We can parse the value in one go
                            if (remaining > state.LongValueLength.Value)
                            {
                                state.CurrentValueMemory = AllocateLongMemory(ref state.DicomDataset, state.LongValueLength.Value);
                                TryRead(ref consumed, ref bufferSpan, ref bufferReader,
                                    state.CurrentValueMemory.Value.Span);
                            }
                            // We cannot parse the value in one go, prep memory and keep track of how much we have left to parse
                            else
                            {
                                state.CurrentValueMemory = AllocateLongMemory(ref state.DicomDataset, state.LongValueLength.Value);
                                state.CurrentLongValueMemoryOffset = remaining;
                                TryRead(ref consumed, ref bufferSpan, ref bufferReader,
                                    state.CurrentValueMemory.Value.Span.Slice(0, remaining));
                                // We still have more bytes to read
                                return consumed;
                            }
                        }
                        // some memory was already allocated for this value
                        else
                        {
                            // We are parsing the value in bits and pieces
                            int bytesToRead = Math.Min(state.LongValueLength.Value - state.CurrentLongValueMemoryOffset,
                                remaining);
                            TryRead(ref consumed, ref bufferSpan, ref bufferReader, state.CurrentValueMemory.Value.Span.Slice(state.CurrentLongValueMemoryOffset, bytesToRead));
                            state.CurrentLongValueMemoryOffset += bytesToRead;

                            if (state.CurrentLongValueMemoryOffset < state.LongValueLength.Value)
                            {
                                // We still have more bytes to read
                                return consumed;
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
                        state.ParseStage = DicomParseStage.ParseGroup;
                    }
                    else
                    {
                        state.CurrentDicomItem = new DicomItem(state.CurrentGroupNumber, state.CurrentElementNumber,
                            state.CurrentVr, DicomItemContent.Create(state.CurrentValueMemory.Value));
                        state.CurrentValueMemory = null;
                        state.CurrentShortValueMemoryOffset = 0;
                        state.CurrentLongValueMemoryOffset = 0;

                        // If we're currently parsing a SQ item, add the item to the sequence
                        var dataset = state.CurrentSequenceItem ?? state.DicomDataset;

                        state.Logger.LogTrace("Parsed tag {Tag}", state.CurrentDicomItem);
                        if (state.CurrentElementNumber != GroupLengthElement)
                        {
                            dataset.Add(state.CurrentGroupNumber, state.CurrentElementNumber, state.CurrentDicomItem.Value);
                        }
                        state.ParseStage = DicomParseStage.ParseGroup;

                        if (state.CurrentGroupNumber == FileMetaInformationGroup
                            && state.CurrentElementNumber == TransferSyntaxElement
                            && state.CurrentDicomItem.Value.Content.Data!.Value.Span.SequenceEqual(_implicitVRLittleEndianBytes))
                        {
                            state.SetToImplicitVrAfterFileMetaInfo = true;
                        }
                    }

                    break;
                default:
                    throw new DicomException("Unknown parse stage: " + state.ParseStage);
            }
        }

        return consumed;
    }

    static bool IsEmpty(ref ReadOnlySpan<byte> bufferSpan, ref SequenceReader<byte> bufferReader) => !bufferSpan.IsEmpty ? bufferSpan.Length == 0 : bufferReader.End;

    static long GetRemaining(ref ReadOnlySpan<byte> bufferSpan, ref SequenceReader<byte> bufferReader)
    {
        return !bufferSpan.IsEmpty ? bufferSpan.Length : bufferReader.Remaining;
    }

    static bool TryReadExplicitVrLongValueLength(ref int offset, ref ReadOnlySpan<byte> bufferSpan,
        ref SequenceReader<byte> bufferReader, out int output)
    {
        if (!bufferSpan.IsEmpty)
        {
            if (bufferSpan.Length < 6)
            {
                output = default;
                return false;
            }

            output = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(bufferSpan.Slice(2, 6)));
            bufferSpan = bufferSpan.Slice(6);
            offset += 6;

            if (!BitConverter.IsLittleEndian)
            {
                output = BinaryPrimitives.ReverseEndianness(output);
            }

            return true;
        }

        if (bufferReader.Remaining < 6)
        {
            output = default;
            return false;
        }

        bufferReader.Advance(2);
        if (bufferReader.TryReadLittleEndian(out output))
        {
            offset += 6;
            return true;
        }

        return false;
    }

    static bool TryReadImplicitVrLongValueLength(ref int offset, ref ReadOnlySpan<byte> bufferSpan,
        ref SequenceReader<byte> bufferReader, out int output)
    {
        if (!bufferSpan.IsEmpty)
        {
            if (bufferSpan.Length < 4)
            {
                output = default;
                return false;
            }

            output = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(bufferSpan));
            bufferSpan = bufferSpan.Slice(4);
            offset += 4;

            if (!BitConverter.IsLittleEndian)
            {
                output = BinaryPrimitives.ReverseEndianness(output);
            }

            return true;
        }

        if (bufferReader.Remaining < 4)
        {
            output = default;
            return false;
        }

        if (bufferReader.TryReadLittleEndian(out output))
        {
            offset += 4;
            return true;
        }

        return false;
    }

    static bool TryReadShortValueLength(ref int offset, ref ReadOnlySpan<byte> bufferSpan, ref SequenceReader<byte> bufferReader, out short output)
    {
        return TryReadShort(ref offset, ref bufferSpan, ref bufferReader, out output);
    }

    static bool TryReadShort(ref int offset, ref ReadOnlySpan<byte> bufferSpan, ref SequenceReader<byte> bufferReader, out short output)
    {
        if (!bufferSpan.IsEmpty)
        {
            if (bufferSpan.Length < sizeof(short))
            {
                output = default;
                return false;
            }

            output = Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(bufferSpan));
            bufferSpan = bufferSpan.Slice(sizeof(short));
            offset += sizeof(short);

            if (!BitConverter.IsLittleEndian)
            {
                output = BinaryPrimitives.ReverseEndianness(output);
            }

            return true;
        }

        if (bufferReader.TryReadLittleEndian(out output))
        {
            offset += sizeof(short);
            return true;
        }

        return false;
    }

    static bool TryReadInt(ref int offset, ref ReadOnlySpan<byte> bufferSpan, ref SequenceReader<byte> bufferReader, out int output)
    {
        if (!bufferSpan.IsEmpty)
        {
            if (bufferSpan.Length < sizeof(int))
            {
                output = default;
                return false;
            }

            output = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(bufferSpan));
            bufferSpan = bufferSpan.Slice(sizeof(int));
            offset += sizeof(int);

            if (!BitConverter.IsLittleEndian)
            {
                output = BinaryPrimitives.ReverseEndianness(output);
            }

            return true;
        }

        if (bufferReader.TryReadLittleEndian(out output))
        {
            offset += sizeof(int);
            return true;
        }

        return false;
    }

    static bool TryRead(ref int offset, ref ReadOnlySpan<byte> bufferSpan, ref SequenceReader<byte> bufferReader, Span<byte> output)
    {
        if (!bufferSpan.IsEmpty)
        {
            if (output.Length <= bufferSpan.Length)
            {
                bufferSpan.Slice(0, output.Length).CopyTo(output);
                bufferSpan = bufferSpan.Slice(output.Length);
                offset += output.Length;
                return true;
            }

            return false;
        }

        if (bufferReader.TryCopyTo(output))
        {
            offset += output.Length;
            bufferReader.Advance(output.Length);
            return true;
        }

        return false;
    }

    static Memory<byte> AllocateShortMemory(ref DicomMemory? memory, ref int memoryOffset,
        ref DicomDataset dicomDataset, ushort memoryLength)
    {
        // Enough space remains in current memory, just advance the offset
        if (memory is not null && memory.Value.Length - memoryOffset >= memoryLength)
        {
            int oldMemoryOffset = memoryOffset;
            memoryOffset += memoryLength;
            return memory.Value.Memory.Slice(oldMemoryOffset, memoryLength);
        }

        // Not enough space remains, allocate new memory
        memory = new DicomMemory(_shortArrayPool, MinimumMemoryBlockSize);
        memoryOffset = memoryLength;
        dicomDataset.ReleaseOnDispose(memory.Value);
        return memory.Value.Memory.Slice(0, memoryLength);
    }

    static Memory<byte> AllocateLongMemory(ref DicomDataset dicomDataset, int memoryLength)
    {
        var pool = memoryLength < 1_048_576 ? _shortArrayPool : _longArrayPool;
        var memory = new DicomMemory(pool, memoryLength);
        dicomDataset.ReleaseOnDispose(memory);
        return memory.Memory;
    }
}
