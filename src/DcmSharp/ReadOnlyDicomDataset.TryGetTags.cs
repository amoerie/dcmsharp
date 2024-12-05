namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetTags(DicomTag tag, out DicomTag[] values)
        => TryGetTags(tag.Group, tag.Element, out values);

    public bool TryGetTags(ushort group, ushort element, out DicomTag[] values)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = [];
            return false;
        }

        switch (vr)
        {
            case DicomVR.AT:
                return _valueParser.AT.TryParseAll(memory.Value.Span, out values);
        }

        values = [];
        return false;
    }
}
