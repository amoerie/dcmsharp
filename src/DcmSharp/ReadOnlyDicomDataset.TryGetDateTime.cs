namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetDateTime(DicomTag tag, out DateTime value)
        => TryGetDateTime(tag.Group, tag.Element, out value);

    public bool TryGetDateTime(ushort group, ushort element, out DateTime value)
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
