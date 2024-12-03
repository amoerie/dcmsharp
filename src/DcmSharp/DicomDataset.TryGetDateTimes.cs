namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetDateTimes(DicomTag tag, out DateTime[] values)
        => TryGetDateTimes(tag.Group, tag.Element, out values);

    public bool TryGetDateTimes(ushort group, ushort element, out DateTime[] values)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = [];
            return false;
        }

        switch (vr)
        {
            case DicomVR.DT:
                return _valueParser.DT.TryParseAll(memory.Value.Span, out values);
        }

        values = [];
        return false;
    }
}
