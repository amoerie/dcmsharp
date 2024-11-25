namespace DcmParse;

public readonly partial record struct DicomDataset
{
    public bool TryGetDate(DicomTag tag, out DateTime value)
        => TryGetDate(tag.Group, tag.Element, out value);

    public bool TryGetDate(ushort group, ushort element, out DateTime value)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.DT:
                return _valueParser.DT.TryParse(memory.Value.Span, out value);
        }

        value = default;
        return false;
    }
}
