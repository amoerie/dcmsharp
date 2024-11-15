using System.Buffers;

namespace DcmParser;

internal struct DicomParseState
{
    // Reusable buffer for parsing unsigned shorts
    public short shortHolder;
    public int intHolder;

    // The current parse state
    public DicomParseStage parseStage = DicomParseStage.ParseGroup;

    // Whether the current dataset is explicit VR
    // The file meta information group (0002) is always explicit VR, so it is safe to begin with this value set to true
    public bool isExplicitVR = true;
    public bool setToImplicitVrAfterFileMetaInfo = false;

    // The current tag
    public ushort currentGroupNumber = default;
    public ushort currentElementNumber = default;

    // The current VR
    public DicomVR currentVr = default!;

    /* The current value length, can be 16 or 32 bits, depending on the VR */
    public int? longValueLength = default;
    public ushort? shortValueLength = default;

    /* The current memory owner that we're biting pieces off of, tracked with an offset */
    public DicomMemory? memory = null;
    public int memoryOffset = 0;

    /* The current memory that we're writing to, tracked with an offset */
    public Memory<byte>? currentValueMemory = null;
    public ushort currentShortValueMemoryOffset = 0;
    public int currentLongValueMemoryOffset = 0;

    /* The current sequence that we're writing to */
    public ushort currentSequenceGroupNumber = default;
    public ushort currentSequenceElementNumber = default;
    public List<DicomDataset>? currentSequenceItems = null;
    public DicomDataset? currentSequenceItem = null;

    /* The current pixel data fragment sequence that we're writing to */
    public ushort currentFragmentsGroupNumber = default;
    public ushort currentFragmentsElementNumber = default;
    public DicomVR currentFragmentsVR = default;
    public List<Memory<byte>>? currentFragments = null;

    /* The current DICOM item */
    public DicomItem? currentDicomItem = null;

    /* The DICOM data set */
    public DicomDataset dicomDataset = default!;

    public DicomParseState()
    {

    }


}
