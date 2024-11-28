namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetULong(DicomTag tag, out ulong value) =>
        TryGetULong(tag.Group, tag.Element, out value);

    public bool TryGetULong(ushort group, ushort element, out ulong value)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.IS:
                if (_valueParser.IS.TryParse(memory.Value.Span, out int isValue)
                    && isValue >= 0)
                {
                    value = (ulong) isValue;
                    return true;
                }

                break;
            case DicomVR.SL:
                if (_valueParser.SL.TryParse(memory.Value.Span, out int slValue)
                    && slValue >= 0)
                {
                    value = (ulong)slValue;
                    return true;
                }

                break;
            case DicomVR.SS:
                if (_valueParser.SS.TryParse(memory.Value.Span, out short ssValue)
                    && ssValue >= 0)
                {
                    value = (ulong) ssValue;
                    return true;
                }

                break;
            case DicomVR.SV:
                if (_valueParser.SV.TryParse(memory.Value.Span, out long svValue)
                    && svValue >= 0)
                {
                    value = (ulong)svValue;
                    return true;
                }

                break;
            case DicomVR.UL:
                if (_valueParser.UL.TryParse(memory.Value.Span, out uint uintValue))
                {
                    value = uintValue;
                    return true;
                }

                break;
            case DicomVR.US:
                if (_valueParser.US.TryParse(memory.Value.Span, out ushort usValue))
                {
                    value = usValue;
                    return true;
                }

                break;
            case DicomVR.UV:
                return _valueParser.UV.TryParse(memory.Value.Span, out value);
        }

        value = default;
        return false;
    }
}
