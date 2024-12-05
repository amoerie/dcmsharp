namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetLong(DicomTag tag, out long value) =>
        TryGetLong(tag.Group, tag.Element, out value);

    public bool TryGetLong(ushort group, ushort element, out long value)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
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
        }

        value = default;
        return false;
    }
}
