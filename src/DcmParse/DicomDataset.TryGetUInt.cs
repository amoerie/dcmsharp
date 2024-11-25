namespace DcmParse;

public readonly partial record struct DicomDataset
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
            case DicomVR.IS:
                if (_valueParser.IS.TryParse(memory.Value.Span, out int isValue)
                    && isValue >= 0)
                {
                    value = (uint) isValue;
                    return true;
                }

                break;
            case DicomVR.SL:
                if (_valueParser.SL.TryParse(memory.Value.Span, out int slValue)
                    && slValue >= 0)
                {
                    value = (uint) slValue;
                    return true;
                }

                break;
            case DicomVR.SS:
                if (_valueParser.SS.TryParse(memory.Value.Span, out short ssValue)
                    && ssValue >= 0)
                {
                    value = (uint) ssValue;
                    return true;
                }

                break;
            case DicomVR.SV:
                if (_valueParser.SV.TryParse(memory.Value.Span, out long svValue)
                    && svValue is >= 0 and <= uint.MaxValue)
                {
                    value = (uint)svValue;
                    return true;
                }

                break;
            case DicomVR.UL:
                return _valueParser.UL.TryParse(memory.Value.Span, out value);
            case DicomVR.US:
                if (_valueParser.US.TryParse(memory.Value.Span, out ushort usValue))
                {
                    value = usValue;
                    return true;
                }

                break;
            case DicomVR.UV:
                if (_valueParser.UV.TryParse(memory.Value.Span, out ulong uvValue)
                    && uvValue <= uint.MaxValue)
                {
                    value = (uint) uvValue;
                    return true;
                }

                break;
        }

        value = default;
        return false;
    }
}
