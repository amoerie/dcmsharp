using System.Runtime.CompilerServices;

namespace DcmParser;

/// <summary>
/// Represents a DICOM value representation (VR).
/// </summary>
public enum DicomVR: byte
{
    AE, // Application Entity
    AS, // Age String
    AT, // Attribute Tag
    CS, // Code String
    DA, // Date
    DS, // Decimal String
    DT, // DateTime
    FL, // Floating Point Single
    FD, // Floating Point Double
    IS, // Integer String
    LO, // Long String
    LT, // Long Text
    OB, // Other Byte
    OD, // Other Double
    OF, // Other Float
    OL, // Other Long
    OW, // Other Word
    OV, // Other Very Long
    PN, // Person Name
    SH, // Short String
    SL, // Signed Long
    SQ, // Sequence of Items
    SS, // Signed Short
    ST, // Short Text
    SV, // Signed Very Long
    TM, // Time
    UC, // Unlimited Characters
    UI, // Unique Identifier (UID)
    UL, // Unsigned Long
    UN, // Unknown
    UR, // URI/URL
    US, // Unsigned Short
    UT, // Unlimited Text,
    UV  // Unsigned Very Long
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
