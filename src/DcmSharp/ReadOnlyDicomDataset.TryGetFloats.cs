namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetFloats(DicomTag tag, out float[] values) =>
        TryGetFloats(tag.Group, tag.Element, out values);

    public bool TryGetFloats(ushort group, ushort element, out float[] values)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = [];
            return false;
        }

        switch (vr)
        {
            case DicomVR.DS:
                return _valueParser.DS.TryParseAll(memory.Value.Span, out values);
            case DicomVR.FL:
                return _valueParser.FL.TryParseAll(memory.Value.Span, out values);
            case DicomVR.IS:
                return _valueParser.IS.TryParseAll(memory.Value.Span, out values);
            case DicomVR.SL:
                return _valueParser.SL.TryParseAll(memory.Value.Span, out values);
            case DicomVR.SS:
                return _valueParser.SS.TryParseAll(memory.Value.Span, out values);
            case DicomVR.SV:
                return _valueParser.SV.TryParseAll(memory.Value.Span, out values);
            case DicomVR.UL:
                return _valueParser.UL.TryParseAll(memory.Value.Span, out values);
            case DicomVR.US:
                return _valueParser.US.TryParseAll(memory.Value.Span, out values);
            case DicomVR.UV:
                return _valueParser.UV.TryParseAll(memory.Value.Span, out values);
        }

        values = [];
        return false;
    }
}
