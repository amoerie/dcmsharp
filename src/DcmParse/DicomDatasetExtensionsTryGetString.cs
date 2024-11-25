using System.Diagnostics.CodeAnalysis;
using System.Text;
using DcmParse.ValueRepresentations;

namespace DcmParse;

public static class DicomDatasetExtensionsTryGetString
{
    public static bool TryGetString(this DicomDataset dataset, DicomTag tag, [NotNullWhen(true)] out string? value)
        => TryGetString(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetString(this DicomDataset dataset, ushort group, ushort element, [NotNullWhen(true)] out string? value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = null;
            return false;
        }

        switch (vr)
        {
            case DicomVR.AE:
                return memory.Value.Span.TryGetAE(out value);
            case DicomVR.AS:
                break;
            case DicomVR.AT:
                break;
            case DicomVR.CS:
                break;
            case DicomVR.DA:
                break;
            case DicomVR.DS:
                break;
            case DicomVR.DT:
                break;
            case DicomVR.FL:
                break;
            case DicomVR.FD:
                break;
            case DicomVR.IS:
                break;
            case DicomVR.LO:
                break;
            case DicomVR.LT:
                break;
            case DicomVR.OB:
                break;
            case DicomVR.OD:
                break;
            case DicomVR.OF:
                break;
            case DicomVR.OL:
                break;
            case DicomVR.OW:
                break;
            case DicomVR.OV:
                break;
            case DicomVR.PN:
                break;
            case DicomVR.SH:
                break;
            case DicomVR.SL:
                break;
            case DicomVR.SQ:
                break;
            case DicomVR.SS:
                break;
            case DicomVR.ST:
                break;
            case DicomVR.SV:
                break;
            case DicomVR.TM:
                break;
            case DicomVR.UC:
                break;
            case DicomVR.UI:
                break;
            case DicomVR.UL:
                break;
            case DicomVR.UN:
                break;
            case DicomVR.UR:
                break;
            case DicomVR.US:
                break;
            case DicomVR.UT:
                break;
            case DicomVR.UV:
                break;
        }

        return true;
    }
}
