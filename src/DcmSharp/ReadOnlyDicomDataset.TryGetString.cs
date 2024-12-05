using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetString(DicomTag tag, [NotNullWhen(true)] out string? value)
        => TryGetString(tag.Group, tag.Element, out value);

    public bool TryGetString(ushort group, ushort element, [NotNullWhen(true)] out string? value)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        var encoding = Encoding;

        switch (vr)
        {
            case DicomVR.AE:
                return _valueParser.AE.TryParse(memory.Value.Span, out value);
            case DicomVR.AS:
                return _valueParser.AS.TryParse(memory.Value.Span, out value);
            case DicomVR.AT:
                return _valueParser.AT.TryParse(memory.Value.Span, out value);
            case DicomVR.CS:
                return _valueParser.CS.TryParse(memory.Value.Span, out value);
            case DicomVR.DA:
                return _valueParser.DA.TryParse(memory.Value.Span, out value);
            case DicomVR.DS:
                return _valueParser.DS.TryParse(memory.Value.Span, out value);
            case DicomVR.DT:
                return _valueParser.DT.TryParse(memory.Value.Span, out value);
            case DicomVR.FL:
                return _valueParser.FL.TryParse(memory.Value.Span, out value);
            case DicomVR.FD:
                return _valueParser.FD.TryParse(memory.Value.Span, out value);
            case DicomVR.IS:
                return _valueParser.IS.TryParse(memory.Value.Span, out value);
            case DicomVR.LO:
                return _valueParser.LO.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.LT:
                return _valueParser.LT.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.PN:
                return _valueParser.PN.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.SH:
                return _valueParser.SH.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.SL:
                return _valueParser.SL.TryParse(memory.Value.Span, out value);
            case DicomVR.SS:
                return _valueParser.SS.TryParse(memory.Value.Span, out value);
            case DicomVR.ST:
                return _valueParser.ST.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.SV:
                return _valueParser.SV.TryParse(memory.Value.Span, out value);
            case DicomVR.TM:
                return _valueParser.TM.TryParse(memory.Value.Span, out value);
            case DicomVR.UC:
                return _valueParser.UC.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.UI:
                return _valueParser.UI.TryParse(memory.Value.Span, out value);
            case DicomVR.UL:
                return _valueParser.UL.TryParse(memory.Value.Span, out value);
            case DicomVR.US:
                return _valueParser.US.TryParse(memory.Value.Span, out value);
            case DicomVR.UT:
                return _valueParser.UT.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.UV:
                return _valueParser.UV.TryParse(memory.Value.Span, out value);
        }

        value = default;
        return false;
    }
}
