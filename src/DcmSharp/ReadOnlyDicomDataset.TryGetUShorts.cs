namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetUShorts(DicomTag tag, out ushort[] values) =>
        TryGetUShorts(tag.Group, tag.Element, out values);

    public bool TryGetUShorts(ushort group, ushort element, out ushort[] values)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = [];
            return false;
        }

        switch (vr)
        {
            case DicomVR.US:
                return _valueParser.US.TryParseAll(memory.Value.Span, out values);
        }

        values = [];
        return false;
    }
}
