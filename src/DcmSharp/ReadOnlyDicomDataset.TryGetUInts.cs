namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetUInts(DicomTag tag, out uint[] values) =>
        TryGetUInts(tag.Group, tag.Element, out values);

    public bool TryGetUInts(ushort group, ushort element, out uint[] values)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = [];
            return false;
        }

        switch (vr)
        {
            case DicomVR.UL:
                return _valueParser.UL.TryParseAll(memory.Value.Span, out values);
            case DicomVR.US:
                return _valueParser.US.TryParseAll(memory.Value.Span, out values);
        }

        values = [];
        return false;
    }
}
