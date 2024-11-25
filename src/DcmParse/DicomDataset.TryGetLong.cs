namespace DcmParse;

public readonly partial record struct DicomDataset
{
    public bool TryGetLong(DicomTag tag, out long value) =>
        TryGetLong(tag.Group, tag.Element, out value);

    public bool TryGetLong(ushort group, ushort element, out long value)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.IS:
                if (_valueParser.IS.TryParse(memory.Value.Span, out int isValue))
                {
                    value = isValue;
                    return true;
                }

                break;
            case DicomVR.SL:
                if (_valueParser.SL.TryParse(memory.Value.Span, out int slValue))
                {
                    value = slValue;
                    return true;
                }

                break;
            case DicomVR.SS:
                if (_valueParser.SS.TryParse(memory.Value.Span, out short ssValue))
                {
                    value = ssValue;
                    return true;
                }

                break;
            case DicomVR.SV:
                return _valueParser.SV.TryParse(memory.Value.Span, out value);
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
                if (_valueParser.UV.TryParse(memory.Value.Span, out ulong uvValue)
                    && uvValue <= long.MaxValue)
                {
                    value = (long) uvValue;
                    return true;
                }

                break;
        }

        value = default;
        return false;
    }
}
