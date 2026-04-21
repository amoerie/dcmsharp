using System.Globalization;

namespace DcmSharp;

public static partial class DicomItemFactory
{
    public static IDicomItem Create(DicomTag tag, int value)
    {
        return Create(tag, tag.ValueRepresentation, value);
    }

    public static IDicomItem Create(DicomTag tag, DicomVR vr, int value)
    {
        ArgumentNullException.ThrowIfNull(tag);

        ushort group = tag.Group;
        ushort element = tag.Element;

        switch (vr)
        {
            case DicomVR.IS:
                return new DicomIntegerString(group, element, [ value.ToString(CultureInfo.InvariantCulture) ]);
            case DicomVR.SL:
                return new DicomSignedLong(group, element, [ value ]);
            case DicomVR.SV:
                return new DicomSignedVeryLong(group, element, [ value ]);
            case DicomVR.US:
                return new DicomUnsignedShort(group, element, [ (ushort)value ]);
            default:
                throw new DicomException($"Creating a DICOM item with VR {vr} with a value of type 'int' is not supported");
        }
    }
}
