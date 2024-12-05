namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetULongs(DicomTag tag, out ulong[] values) =>
        TryGetULongs(tag.Group, tag.Element, out values);

    public bool TryGetULongs(ushort group, ushort element, out ulong[] values)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
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
            case DicomVR.UV:
                return _valueParser.UV.TryParseAll(memory.Value.Span, out values);
        }

        values = [];
        return false;
    }
}
