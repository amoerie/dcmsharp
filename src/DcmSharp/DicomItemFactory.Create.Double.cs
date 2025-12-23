namespace DcmSharp;

public static partial class DicomItemFactory
{
    public static IDicomItem Create(DicomTag tag, double value)
    {
        return Create(tag, tag.ValueRepresentation, value);
    }

    public static IDicomItem Create(DicomTag tag, DicomVR vr, double value)
    {
        ArgumentNullException.ThrowIfNull(tag);

        ushort group = tag.Group;
        ushort element = tag.Element;

        switch (vr)
        {
            case DicomVR.AE:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.AE)} with a value of type 'double' is not supported");
            case DicomVR.AS:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.AS)} with a value of type 'double' is not supported");
            case DicomVR.AT:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.AT)} with a value of type 'double' is not supported");
            case DicomVR.CS:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.CS)} with a value of type 'double' is not supported");
            case DicomVR.DA:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.DA)} with a value of type 'double' is not supported");
            case DicomVR.DS:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.DS)} with a value of type 'double' is not supported");
            case DicomVR.DT:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.DT)} with a value of type 'double' is not supported");
            case DicomVR.FL:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.FL)} with a value of type 'double' is not supported");
            case DicomVR.FD:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.FD)} with a value of type 'double' is not supported");
            case DicomVR.IS:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.IS)} with a value of type 'double' is not supported");
            case DicomVR.LO:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.LO)} with a value of type 'double' is not supported");
            case DicomVR.LT:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.LT)} with a value of type 'double' is not supported");
            case DicomVR.OB:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OB)} with a value of type 'double' is not supported");
            case DicomVR.OD:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OD)} with a value of type 'double' is not supported");
            case DicomVR.OF:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OF)} with a value of type 'double' is not supported");
            case DicomVR.OL:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OL)} with a value of type 'double' is not supported");
            case DicomVR.OW:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OW)} with a value of type 'double' is not supported");
            case DicomVR.OV:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OV)} with a value of type 'double' is not supported");
            case DicomVR.PN:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.PN)} with a value of type 'double' is not supported");
            case DicomVR.SH:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.SH)} with a value of type 'double' is not supported");
            case DicomVR.SL:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.SL)} with a value of type 'double' is not supported");
            case DicomVR.SQ:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.SQ)} with a value of type 'double' is not supported");
            case DicomVR.SS:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.SS)} with a value of type 'double' is not supported");
            case DicomVR.ST:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.ST)} with a value of type 'double' is not supported");
            case DicomVR.SV:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.SV)} with a value of type 'double' is not supported");
            case DicomVR.TM:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.TM)} with a value of type 'double' is not supported");
            case DicomVR.UC:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.UC)} with a value of type 'double' is not supported");
            case DicomVR.UI:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.UI)} with a value of type 'double' is not supported");
            case DicomVR.UL:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.UL)} with a value of type 'double' is not supported");
            case DicomVR.UN:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.UN)} with a value of type 'double' is not supported");
            case DicomVR.UR:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.UR)} with a value of type 'double' is not supported");
            case DicomVR.US:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.US)} with a value of type 'double' is not supported");
            case DicomVR.UT:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.UT)} with a value of type 'double' is not supported");
            case DicomVR.UV:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.UV)} with a value of type 'double' is not supported");
            default:
                throw new DicomException($"Creating a DICOM item with VR {vr} with a value of type 'double' is not supported");
        }
    }
}
