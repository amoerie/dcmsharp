namespace DcmSharp;

public static partial class DicomItemFactory
{
    public static IDicomItem Create(DicomTag tag, string value)
    {
        return Create(tag, tag.ValueRepresentation, value);
    }

    public static IDicomItem Create(DicomTag tag, DicomVR vr, string value)
    {
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentNullException.ThrowIfNull(value);

        ushort group = tag.Group;
        ushort element = tag.Element;

        switch (vr)
        {
            case DicomVR.AE:
                return new DicomApplicationEntity(group, element, [value]);
            case DicomVR.AS:
                return new DicomAgeString(group, element, [value]);
            case DicomVR.AT:
                return new DicomAttributeTag(group, element, [value]);
            case DicomVR.CS:
                return new DicomCodeString(group, element, [value]);
            case DicomVR.DS:
                return new DicomDecimalString(group, element, [value]);
            case DicomVR.IS:
                return new DicomIntegerString(group, element, [value]);
            case DicomVR.LO:
                return new DicomLongString(group, element, [value]);
            case DicomVR.SH:
                return new DicomShortString(group, element, [value]);
            case DicomVR.ST:
                return new DicomShortText(group, element, value);
            case DicomVR.UC:
                return new DicomUnlimitedCharacters(group, element, [value]);
            case DicomVR.UI:
                return new DicomShortString(group, element, [value]);
            case DicomVR.UR:
                return new DicomUrl(group, element, [value]);
            case DicomVR.UT:
                return new DicomUnlimitedText(group, element, value);
            case DicomVR.UV:
                return new DicomUniversalResource(group, element, value);
            default:
                throw new DicomException($"Creating a DICOM item with VR {vr} with a value of type 'string' is not supported");
        }
    }
}
