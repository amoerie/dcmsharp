namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetULong(DicomTag tag, out ulong value) =>
        TryGetULong(tag.Group, tag.Element, out value);

    public bool TryGetULong(ushort group, ushort element, out ulong value)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
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
