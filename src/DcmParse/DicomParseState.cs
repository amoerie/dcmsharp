using Microsoft.Extensions.Logging;

namespace DcmParse;

internal struct DicomParseState
{
    // The logger to use
    public ILogger Logger = default!;

    // Reusable buffer for parsing unsigned shorts
    public short ShortHolder;
    public int IntHolder;

    // The current parse state
    public DicomParseStage ParseStage = DicomParseStage.ParseGroup;

    // Whether the current dataset is explicit VR
    // The file meta information group (0002) is always explicit VR, so it is safe to begin with this value set to true
    public bool IsExplicitVR = true;
    public bool SetToImplicitVrAfterFileMetaInfo = false;

    // The current tag
    public ushort CurrentGroupNumber = default;
    public ushort CurrentElementNumber = default;

    // The current VR
    public DicomVR CurrentVr = default!;

    /* The current value length, can be 16 or 32 bits, depending on the VR */
    public int? LongValueLength = default;
    public ushort? ShortValueLength = default;

    /* The current memory owner that we're biting pieces off of, tracked with an offset */
    public DicomMemory? Memory = null;
    public int MemoryOffset = 0;

    /* The current memory that we're writing to, tracked with an offset */
    public Memory<byte>? CurrentValueMemory = null;
    public ushort CurrentShortValueMemoryOffset = 0;
    public int CurrentLongValueMemoryOffset = 0;

    /* The current sequence that we're writing to */
    public DicomSequence? CurrentSequence = null;
    public DicomDataset? CurrentSequenceItem = null;
    public readonly Stack<DicomSequence> CurrentSequences = new Stack<DicomSequence>();
    public readonly Stack<DicomDataset> CurrentSequenceItems = new Stack<DicomDataset>();

    /* The current pixel data fragment sequence that we're writing to */
    public ushort CurrentFragmentsGroupNumber = default;
    public ushort CurrentFragmentsElementNumber = default;
    public DicomVR CurrentFragmentsVR = default;
    public List<Memory<byte>>? CurrentFragments = null;

    /* The current DICOM item */
    public DicomItem? CurrentDicomItem = null;

    /* The DICOM data set */
    public DicomDataset DicomDataset = default!;

    public DicomParseState()
    {

    }


}
