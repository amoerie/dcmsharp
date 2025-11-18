using System.Globalization;

namespace DcmSharp;

public static partial class DicomItemFactory
{
    public static IDicomItem Create(DicomTag tag, DateOnly value)
    {
        return Create(tag, tag.ValueRepresentation, value);
    }

    public static IDicomItem Create(DicomTag tag, DicomVR vr, DateOnly value)
    {
        ArgumentNullException.ThrowIfNull(tag);

        ushort group = tag.Group;
        ushort element = tag.Element;

        switch (vr)
        {
            case DicomVR.DA:
                return new DicomDate(group, element, [ value ]);
            case DicomVR.DT:
                return new DicomDateTime(group, element, [ new DateTime(value, new TimeOnly()) ]);
            default:
                throw new DicomException($"Creating a DICOM item with VR {vr} with a value of type 'DateOnly' is not supported");
        }
    }
}
