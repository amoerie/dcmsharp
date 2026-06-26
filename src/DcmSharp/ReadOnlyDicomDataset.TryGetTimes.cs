namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetTimes(DicomTag tag, out TimeOnly[] value) =>
        TryGetTimes(tag.Group, tag.Element, out value);

    public bool TryGetTimes(ushort group, ushort element, out TimeOnly[] value)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = [];
            return false;
        }

        switch (vr)
        {
            case DicomVR.TM:
                return _valueParser.TM.TryParseAll(memory.Value.Span, out value);
        }

        value = [];
        return false;
    }
}
