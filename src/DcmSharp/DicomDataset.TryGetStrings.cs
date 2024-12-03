namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetStrings(DicomTag tag, out string[] values)
        => TryGetStrings(tag.Group, tag.Element, out values);

    public bool TryGetStrings(ushort group, ushort element, out string[] values)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = [];
            return false;
        }

        var encoding = Encoding;

        switch (vr)
        {
            case DicomVR.AE:
                return _valueParser.AE.TryParseAll(memory.Value.Span, out values);
            case DicomVR.AS:
                return _valueParser.AS.TryParseAll(memory.Value.Span, out values);
            case DicomVR.AT:
                return _valueParser.AT.TryParseAll(memory.Value.Span, out values);
            case DicomVR.CS:
                return _valueParser.CS.TryParseAll(memory.Value.Span, out values);
            case DicomVR.DA:
                return _valueParser.DA.TryParseAll(memory.Value.Span, out values);
            case DicomVR.DS:
                return _valueParser.DS.TryParseAll(memory.Value.Span, out values);
            case DicomVR.DT:
                return _valueParser.DT.TryParseAll(memory.Value.Span, out values);
            case DicomVR.FL:
                return _valueParser.FL.TryParseAll(memory.Value.Span, out values);
            case DicomVR.FD:
                return _valueParser.FD.TryParseAll(memory.Value.Span, out values);
            case DicomVR.IS:
                return _valueParser.IS.TryParseAll(memory.Value.Span, out values);
            case DicomVR.LO:
                return _valueParser.LO.TryParseAll(memory.Value.Span, encoding, out values);
            case DicomVR.LT:
                {
                    if (_valueParser.LT.TryParse(memory.Value.Span, encoding, out string? value))
                    {
                        values = [value];
                        return true;
                    }

                    break;
                }
            case DicomVR.PN:
                return _valueParser.PN.TryParseAll(memory.Value.Span, encoding, out values);
            case DicomVR.SH:
                return _valueParser.SH.TryParseAll(memory.Value.Span, encoding, out values);
            case DicomVR.SL:
                return _valueParser.SL.TryParseAll(memory.Value.Span, out values);
            case DicomVR.SS:
                return _valueParser.SS.TryParseAll(memory.Value.Span, out values);
            case DicomVR.ST:
                {
                    if (_valueParser.ST.TryParse(memory.Value.Span, encoding, out string? value))
                    {
                        values = [value];
                        return true;
                    }

                    break;
                }
            case DicomVR.SV:
                return _valueParser.SV.TryParseAll(memory.Value.Span, out values);
            case DicomVR.TM:
                return _valueParser.TM.TryParseAll(memory.Value.Span, out values);
            case DicomVR.UC:
                return _valueParser.UC.TryParseAll(memory.Value.Span, encoding, out values);
            case DicomVR.UI:
                return _valueParser.UI.TryParseAll(memory.Value.Span, out values);
            case DicomVR.UL:
                return _valueParser.UL.TryParseAll(memory.Value.Span, out values);
            case DicomVR.US:
                return _valueParser.US.TryParseAll(memory.Value.Span, out values);
            case DicomVR.UT:
                {
                    if (_valueParser.UT.TryParse(memory.Value.Span, encoding, out string? value))
                    {
                        values = [value];
                        return true;
                    }

                    break;
                }
            case DicomVR.UV:
                return _valueParser.UV.TryParseAll(memory.Value.Span, out values);
        }

        values = [];
        return false;
    }
}
