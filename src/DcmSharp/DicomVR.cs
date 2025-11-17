using System.Runtime.CompilerServices;

namespace DcmSharp;

/// <summary>
/// Represents a DICOM value representation (VR).
/// </summary>
public enum DicomVR : byte
{
    /// <summary>Application Entity</summary>
    AE,

    /// <summary>Age String</summary>
    AS,

    /// <summary>Attribute Tag</summary>
    AT,

    /// <summary>Code String</summary>
    CS,

    /// <summary>Date</summary>
    DA,

    /// <summary>Decimal String</summary>
    DS,

    /// <summary>DateTime</summary>
    DT,

    /// <summary>Floating Point Single</summary>
    FL,

    /// <summary>Floating Point Double</summary>
    FD,

    /// <summary>Integer String</summary>
    IS,

    /// <summary>Long String</summary>
    LO,

    /// <summary>Long Text</summary>
    LT,

    /// <summary>Other Byte</summary>
    OB,

    /// <summary>Other Double</summary>
    OD,

    /// <summary>Other Float</summary>
    OF,

    /// <summary>Other Long</summary>
    OL,

    /// <summary>Other Word</summary>
    OW,

    /// <summary>Other Very Long</summary>
    OV,

    /// <summary>Person Name</summary>
    PN,

    /// <summary>Short String</summary>
    SH,

    /// <summary>Signed Long</summary>
    SL,

    /// <summary>Sequence of Items</summary>
    SQ,

    /// <summary>Signed Short</summary>
    SS,

    /// <summary>Short Text</summary>
    ST,

    /// <summary>Signed Very Long</summary>
    SV,

    /// <summary>Time</summary>
    TM,

    /// <summary>Unlimited Characters</summary>
    UC,

    /// <summary>Unique Identifier (UID)</summary>
    UI,

    /// <summary>Unsigned Long</summary>
    UL,

    /// <summary>Unknown</summary>
    UN,

    /// <summary>URI/URL</summary>
    UR,

    /// <summary>Unsigned Short</summary>
    US,

    /// <summary>Unlimited Text,</summary>
    UT,

    /// <summary>Unsigned Very Long</summary>
    UV
}

public static class ExtensionsForDicomVR
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is32BitLength(this DicomVR vr) => vr is
        DicomVR.OB
        or DicomVR.OD
        or DicomVR.OW
        or DicomVR.OF
        or DicomVR.OL
        or DicomVR.OV
        or DicomVR.SQ
        or DicomVR.SV
        or DicomVR.UC
        or DicomVR.UN
        or DicomVR.UR
        or DicomVR.UT
        or DicomVR.UV;
}
