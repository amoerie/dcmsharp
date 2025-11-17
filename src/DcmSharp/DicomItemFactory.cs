using System.Globalization;

namespace DcmSharp;

public static class DicomItemFactory
{
    public static DicomItem CreateFromString(DicomTag tag, string value)
    {
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentNullException.ThrowIfNull(value);

        switch (tag.VR)
        {
            case DicomVR.AE:
                return new DicomApplicationEntity(tag, value);
            case DicomVR.AS:
                return new DicomAgeString(tag, value);
            case DicomVR.AT:
                return new DicomAttributeTag(tag, value);
            case DicomVR.CS:
                return new DicomCodeString(tag, value);
            case DicomVR.DA:
                return new DicomDate(tag, DateOnly.ParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture));
            case DicomVR.DS:
                return new DicomDecimalString(tag, value);
            case DicomVR.DT:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.DT)} with a value of type 'string' is not supported");
            case DicomVR.FL:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.FL)} with a value of type 'string' is not supported");
            case DicomVR.FD:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.FD)} with a value of type 'string' is not supported");
            case DicomVR.IS:
                return new DicomIntegerString(tag, value);
            case DicomVR.LO:
                return new DicomLongString(tag, value);
            case DicomVR.LT:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.LT)} with a value of type 'string' is not supported");
            case DicomVR.OB:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OB)} with a value of type 'string' is not supported");
            case DicomVR.OD:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OD)} with a value of type 'string' is not supported");
            case DicomVR.OF:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OF)} with a value of type 'string' is not supported");
            case DicomVR.OL:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OL)} with a value of type 'string' is not supported");
            case DicomVR.OW:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OW)} with a value of type 'string' is not supported");
            case DicomVR.OV:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.OV)} with a value of type 'string' is not supported");
            case DicomVR.PN:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.PN)} with a value of type 'string' is not supported");
            case DicomVR.SH:
                return new DicomShortString(tag, value);
            case DicomVR.SL:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.SL)} with a value of type 'string' is not supported");
            case DicomVR.SQ:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.SQ)} with a value of type 'string' is not supported");
            case DicomVR.SS:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.SS)} with a value of type 'string' is not supported");
            case DicomVR.ST:
                return new DicomShortText(tag, value);
            case DicomVR.SV:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.SV)} with a value of type 'string' is not supported");
            case DicomVR.TM:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.TM)} with a value of type 'string' is not supported");
            case DicomVR.UC:
                return new DicomUnlimitedCharacters(tag, value);
            case DicomVR.UI:
                return new DicomShortString(tag, value);
            case DicomVR.UL:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.UL)} with a value of type 'string' is not supported");
            case DicomVR.UN:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.UN)} with a value of type 'string' is not supported");
            case DicomVR.UR:
                return new DicomUrl(tag, value);
            case DicomVR.US:
                throw new NotSupportedException($"Creating a DICOM item with VR {nameof(DicomVR.US)} with a value of type 'string' is not supported");
            case DicomVR.UT:
                return new DicomUnlimitedText(tag, value);
            case DicomVR.UV:
                return new DicomUniversalResource(tag, value);
            default:
                throw new ArgumentOutOfRangeException(nameof(tag), $"Tag {tag.Description} has unsupported VR: {tag.VR}");
        }
    }
}
