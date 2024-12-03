namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetShorts(DicomTag tag, out short[] values) =>
        TryGetShorts(tag.Group, tag.Element, out values);

    public bool TryGetShorts(ushort group, ushort element, out short[] values)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = [];
            return false;
        }

        switch (vr)
        {
            case DicomVR.SS:
                return _valueParser.SS.TryParseAll(memory.Value.Span, out values);
        }

        values = [];
        return false;
    }
}
