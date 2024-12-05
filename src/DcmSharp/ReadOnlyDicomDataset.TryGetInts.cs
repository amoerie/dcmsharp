namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetInts(DicomTag tag, out int[] values) =>
        TryGetInts(tag.Group, tag.Element, out values);

    public bool TryGetInts(ushort group, ushort element, out int[] values)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
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
            case DicomVR.US:
                return _valueParser.US.TryParseAll(memory.Value.Span, out values);
        }

        values = [];
        return false;
    }
}
