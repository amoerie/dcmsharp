namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public DicomDataset ToDicomDataset()
    {
        var dataset = new DicomDataset();

        foreach (var (_, (group, element, vr, content)) in _items)
        {
            // key = (uint)group << 16 | element
            switch (vr)
            {
                case DicomVR.AE:
                    break;
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
                default:
                    throw new NotSupportedException("Unsupported VR: " + vr);
            }
        }

        return dataset;
    }
}
