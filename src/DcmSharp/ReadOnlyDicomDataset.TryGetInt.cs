namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetInt(DicomTag tag, out int value) =>
        TryGetInt(tag.Group, tag.Element, out value);

    public bool TryGetInt(ushort group, ushort element, out int value)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.IS:
                return _valueParser.IS.TryParse(memory.Value.Span, out value);
            case DicomVR.SL:
                return _valueParser.SL.TryParse(memory.Value.Span, out value);
            case DicomVR.SS:
                return _valueParser.SS.TryParse(memory.Value.Span, out value);
            case DicomVR.US:
                return _valueParser.US.TryParse(memory.Value.Span, out value);
        }

        value = default;
        return false;
    }
}
