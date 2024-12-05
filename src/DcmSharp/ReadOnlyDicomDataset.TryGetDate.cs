namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetDate(DicomTag tag, out DateOnly value)
        => TryGetDate(tag.Group, tag.Element, out value);

    public bool TryGetDate(ushort group, ushort element, out DateOnly value)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.DA:
                return _valueParser.DA.TryParse(memory.Value.Span, out value);
        }

        value = default;
        return false;
    }
}
