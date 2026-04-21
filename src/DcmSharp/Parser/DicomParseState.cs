using System.Diagnostics;
using System.Runtime.InteropServices;
using DcmSharp.Memory;
using Microsoft.Extensions.Logging;

namespace DcmSharp.Parser;

/// <summary>Tracks the end position of an open sequence or sequence item during parsing.</summary>
[StructLayout(LayoutKind.Auto)]
internal readonly record struct SequencePosition(long? EndPosition);

[DebuggerDisplay(
    "[Stage:{ParseStage}] [Tag:({CurrentGroupNumber,h}, {CurrentElementNumber,h})] [Vr:{CurrentVr}]"
)]
internal struct DicomParseState
{
    public ILogger Logger = default!;
    public DicomValueParser ValueParser = default!;

    public long Position;

    public short ShortHolder;
    public int IntHolder;

    public DicomParseStage ParseStage = DicomParseStage.ParseGroup;
    public bool IsStopped = false;

    public bool IsExplicitVR = true;
    public bool SetToImplicitVrAfterFileMetaInfo = false;

    public ushort CurrentGroupNumber = default;
    public ushort CurrentElementNumber = default;
    public DicomVR CurrentVr = default!;

    public int? LongValueLength = default;
    public ushort? ShortValueLength = default;

    /* Memory block being carved up for short values */
    public DicomMemory? Memory = null;
    public int MemoryOffset = 0;

    /* Slice currently being filled during ParseValue */
    public Memory<byte>? CurrentValueMemory = null;
    public ushort CurrentShortValueMemoryOffset = 0;
    public int CurrentLongValueMemoryOffset = 0;

    /* Memory registry for the root dataset — shared with the handler */
    public DicomMemories Memories = default!;

    /* Handler that receives parse events and builds the output */
    public IDicomDatasetHandler Handler = default!;

    /* Sequence position tracking (dataset construction lives in the handler) */
    public SequencePosition? CurrentSequence = null;
    public SequencePosition? CurrentSequenceItem = null;
    public readonly Stack<SequencePosition> CurrentSequences = new Stack<SequencePosition>();
    public readonly Stack<SequencePosition> CurrentSequenceItems = new Stack<SequencePosition>();

    /* Fragment mode flag — dataset construction lives in the handler */
    public bool IsParsingFragments = false;

    public DicomParseState() { }
}
