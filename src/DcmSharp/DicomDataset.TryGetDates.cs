namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetDates(DicomTag tag, out DateOnly[] values)
        => TryGetDates(tag.Group, tag.Element, out values);

    public bool TryGetDates(ushort group, ushort element, out DateOnly[] values)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = [];
            return false;
        }

        switch (vr)
        {
            case DicomVR.DA:
                return _valueParser.DA.TryParseAll(memory.Value.Span, out values);
        }

        values = [];
        return false;
    }
}
