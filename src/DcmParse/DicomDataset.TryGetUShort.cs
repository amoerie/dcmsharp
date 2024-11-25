namespace DcmParse;

public readonly partial record struct DicomDataset
{
    public bool TryGetUShort(DicomTag tag, out ushort value) =>
        TryGetUShort(tag.Group, tag.Element, out value);

    public bool TryGetUShort(ushort group, ushort element, out ushort value)
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
                    && isValue is >= 0 and <= ushort.MaxValue)
                {
                    value = (ushort) isValue;
                    return true;
                }

                break;
            case DicomVR.SL:
                if (_valueParser.SL.TryParse(memory.Value.Span, out int slValue)
                    && slValue is >= 0 and <= ushort.MaxValue)
                {
                    value = (ushort) slValue;
                    return true;
                }

                break;
            case DicomVR.SS:
                if (_valueParser.SS.TryParse(memory.Value.Span, out short ssValue)
                    && ssValue >= 0)
                {
                    value = (ushort) ssValue;
                    return true;
                }

                break;
            case DicomVR.SV:
                if (_valueParser.SV.TryParse(memory.Value.Span, out long svValue)
                    && svValue is >= 0 and <= ushort.MaxValue)
                {
                    value = (ushort) svValue;
                    return true;
                }

                break;
            case DicomVR.UL:
                if (_valueParser.UL.TryParse(memory.Value.Span, out uint uintValue)
                    && uintValue <= ushort.MaxValue)
                {
                    value = (ushort) uintValue;
                    return true;
                }

                break;
            case DicomVR.US:
                return _valueParser.US.TryParse(memory.Value.Span, out value);
            case DicomVR.UV:
                if (_valueParser.UV.TryParse(memory.Value.Span, out ulong uvValue)
                    && uvValue <= ushort.MaxValue)
                {
                    value = (ushort) uvValue;
                    return true;
                }

                break;
        }

        value = default;
        return false;
    }
}
