namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetInts(DicomTag tag, out int[] values) =>
        TryGetInts(tag.Group, tag.Element, out values);

    public bool TryGetInts(ushort group, ushort element, out int[] values)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = [];
            return false;
        }

        switch (vr)
        {
            case DicomVR.IS:
                return _valueParser.IS.TryParseAll(memory.Value.Span, out values);
            case DicomVR.SL:
                // return _valueParser.SL.TryParse(memory.Value.Span, out value);
            case DicomVR.SS:
                // if (_valueParser.SS.TryParse(memory.Value.Span, out short ssValue))
                // {
                //     value = ssValue;
                //     return true;
                // }

                break;
            case DicomVR.SV:
                // if (_valueParser.SV.TryParse(memory.Value.Span, out long svValue)
                //     && svValue is >= int.MinValue and <= int.MaxValue)
                // {
                //     value = (int)svValue;
                //     return true;
                // }

                break;
            case DicomVR.UL:
                // if (_valueParser.UL.TryParse(memory.Value.Span, out uint uintValue)
                //     && uintValue <= int.MaxValue)
                // {
                //     value = (int)uintValue;
                //     return true;
                // }

                break;
            case DicomVR.US:
                // if (_valueParser.US.TryParse(memory.Value.Span, out ushort usValue))
                // {
                //     value = usValue;
                //     return true;
                // }

                break;
            case DicomVR.UV:
                // if (_valueParser.UV.TryParse(memory.Value.Span, out ulong uvValue)
                //     && uvValue <= int.MaxValue)
                // {
                //     value = (int) uvValue;
                //     return true;
                // }

                break;
        }

        values = [];
        return false;
    }
}
