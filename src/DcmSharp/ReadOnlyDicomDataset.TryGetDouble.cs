namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetDouble(DicomTag tag, out double value) =>
        TryGetDouble(tag.Group, tag.Element, out value);

    public bool TryGetDouble(ushort group, ushort element, out double value)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.DS:
                return _valueParser.DS.TryParse(memory.Value.Span, out value);
            case DicomVR.FD:
                return _valueParser.FD.TryParse(memory.Value.Span, out value);
            case DicomVR.FL:
                return _valueParser.FL.TryParse(memory.Value.Span, out value);
            case DicomVR.IS:
                return _valueParser.IS.TryParse(memory.Value.Span, out value);
            case DicomVR.SL:
                return _valueParser.SL.TryParse(memory.Value.Span, out value);
            case DicomVR.SS:
                return _valueParser.SS.TryParse(memory.Value.Span, out value);
            case DicomVR.SV:
                return _valueParser.SV.TryParse(memory.Value.Span, out value);
            case DicomVR.UL:
                return _valueParser.UL.TryParse(memory.Value.Span, out value);
            case DicomVR.US:
                return _valueParser.US.TryParse(memory.Value.Span, out value);
            case DicomVR.UV:
                return _valueParser.UV.TryParse(memory.Value.Span, out value);
        }

        value = default;
        return false;
    }
}
