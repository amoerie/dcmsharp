namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetShort(DicomTag tag, out short value) =>
        TryGetShort(tag.Group, tag.Element, out value);

    public bool TryGetShort(ushort group, ushort element, out short value)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.SS:
                return _valueParser.SS.TryParse(memory.Value.Span, out value);
        }

        value = default;
        return false;
    }
}
