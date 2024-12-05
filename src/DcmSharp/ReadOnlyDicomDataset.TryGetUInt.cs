namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetUInt(DicomTag tag, out uint value) =>
        TryGetUInt(tag.Group, tag.Element, out value);

    public bool TryGetUInt(ushort group, ushort element, out uint value)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.UL:
                return _valueParser.UL.TryParse(memory.Value.Span, out value);
            case DicomVR.US:
                return _valueParser.US.TryParse(memory.Value.Span, out value);
        }

        value = default;
        return false;
    }
}
