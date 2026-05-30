namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetTime(DicomTag tag, out TimeOnly value) =>
        TryGetTime(tag.Group, tag.Element, out value);

    public bool TryGetTime(ushort group, ushort element, out TimeOnly value)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.TM:
                return _valueParser.TM.TryParse(memory.Value.Span, out value);
        }

        value = default;
        return false;
    }
}
