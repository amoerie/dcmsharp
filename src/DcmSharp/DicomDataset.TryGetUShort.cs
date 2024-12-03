namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetUShort(DicomTag tag, out ushort value) =>
        TryGetUShort(tag.Group, tag.Element, out value);

    public bool TryGetUShort(ushort group, ushort element, out ushort value)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.US:
                return _valueParser.US.TryParse(memory.Value.Span, out value);
        }

        value = default;
        return false;
    }
}
