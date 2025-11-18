namespace DcmSharp;

/// <summary>
/// The definition of a DICOM tag
/// </summary>
public sealed partial record DicomTag(
    ushort Group,
    ushort Element,
    DicomVR ValueRepresentation,
    DicomVR[] AdditionalValueRepresentations,
    DicomVM ValueMultiplicity,
    string Keyword,
    string Name)
{
    public override string ToString()
    {
        return $"({Group:x4},{Element:x4}) {Name}";
    }
}
