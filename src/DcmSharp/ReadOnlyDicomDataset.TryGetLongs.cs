namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetLongs(DicomTag tag, out long[] values) =>
        TryGetLongs(tag.Group, tag.Element, out values);

    public bool TryGetLongs(ushort group, ushort element, out long[] values)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = [];
            return false;
        }

        switch (vr)
        {
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
        }

        values = [];
        return false;
    }
}
