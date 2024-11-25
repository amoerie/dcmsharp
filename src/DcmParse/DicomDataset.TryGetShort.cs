namespace DcmParse;

public readonly partial record struct DicomDataset
{
    public bool TryGetShort(DicomTag tag, out short value) =>
        TryGetShort(tag.Group, tag.Element, out value);

    public bool TryGetShort(ushort group, ushort element, out short value)
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
                    && isValue is >= short.MinValue and <= short.MaxValue)
                {
                    value = (short) isValue;
                    return true;
                }

                break;
            case DicomVR.SL:
                if (_valueParser.SL.TryParse(memory.Value.Span, out int slValue)
                    && slValue is >= short.MinValue and <= short.MaxValue)
                {
                    value = (short) slValue;
                    return true;
                }

                break;
            case DicomVR.SS:
                return _valueParser.SS.TryParse(memory.Value.Span, out value);
            case DicomVR.SV:
                if (_valueParser.SV.TryParse(memory.Value.Span, out long svValue)
                    && svValue is >= short.MinValue and <= short.MaxValue)
                {
                    value = (short) svValue;
                    return true;
                }

                break;
            case DicomVR.UL:
                if (_valueParser.UL.TryParse(memory.Value.Span, out uint uintValue)
                    && uintValue <= short.MaxValue)
                {
                    value = (short) uintValue;
                    return true;
                }

                break;
            case DicomVR.US:
                if (_valueParser.US.TryParse(memory.Value.Span, out ushort usValue)
                    && usValue <= short.MaxValue)
                {
                    value = (short) usValue;
                    return true;
                }

                break;
            case DicomVR.UV:
                if (_valueParser.UV.TryParse(memory.Value.Span, out ulong uvValue)
                    && uvValue <= (ulong) short.MaxValue)
                {
                    value = (short) uvValue;
                    return true;
                }

                break;
        }

        value = default;
        return false;
    }
}
