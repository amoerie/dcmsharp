namespace DcmSharp;

public static partial class DicomItemFactory
{
    public static IDicomItem Create(DicomTag tag, ushort value)
    {
        return Create(tag, tag.ValueRepresentation, value);
    }

    public static IDicomItem Create(DicomTag tag, DicomVR vr, ushort value)
    {
        ArgumentNullException.ThrowIfNull(tag);

        ushort group = tag.Group;
        ushort element = tag.Element;

        switch (vr)
        {
            case DicomVR.US:
                return new DicomUnsignedShort(group, element, [value]);
            default:
                throw new DicomException($"Creating a DICOM item with VR {vr} with a value of type 'ushort' is not supported");
        }
    }
}
