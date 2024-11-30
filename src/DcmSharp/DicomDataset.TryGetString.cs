using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetString(DicomTag tag, [NotNullWhen(true)] out string? value)
        => TryGetString(tag.Group, tag.Element, out value);

    public bool TryGetString(ushort group, ushort element, [NotNullWhen(true)] out string? value)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        var encoding = Encoding;

        switch (vr)
        {
            case DicomVR.AE:
                return _valueParser.AE.TryParse(memory.Value.Span, out value);
            case DicomVR.AS:
                return _valueParser.AS.TryParse(memory.Value.Span, out value);
            case DicomVR.AT:
                break;
            case DicomVR.CS:
                return _valueParser.CS.TryParse(memory.Value.Span, out value);
            case DicomVR.DA:
                // TODO return raw value
                if (_valueParser.DA.TryParse(memory.Value.Span, out DateOnly daValue))
                {
                    value = daValue.ToString("O");
                    return true;
                }

                break;
            case DicomVR.DS:
                return _valueParser.DS.TryParseString(memory.Value.Span, out value);
            case DicomVR.DT:
                // TODO return raw value
                if (_valueParser.DT.TryParse(memory.Value.Span, out DateTime dtValue))
                {
                    value = dtValue.ToString("O");
                    return true;
                }

                break;
            case DicomVR.FL:
                if(_valueParser.FL.TryParse(memory.Value.Span, out float flValue))
                {
                    value = flValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                break;
            case DicomVR.FD:
                if(_valueParser.FD.TryParse(memory.Value.Span, out double fdValue))
                {
                    value = fdValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                break;
            case DicomVR.IS:
                return _valueParser.IS.TryParseString(memory.Value.Span, out value);
            case DicomVR.LO:
                return _valueParser.LO.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.LT:
                return _valueParser.LT.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.PN:
                return _valueParser.PN.TryParseString(memory.Value.Span, encoding, out value);
            case DicomVR.SH:
                return _valueParser.SH.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.SL:
                if(_valueParser.SL.TryParse(memory.Value.Span, out int slValue))
                {
                    value = slValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                break;
            case DicomVR.SS:
                if(_valueParser.SS.TryParse(memory.Value.Span, out short ssValue))
                {
                    value = ssValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                break;
            case DicomVR.ST:
                return _valueParser.ST.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.SV:
                if(_valueParser.SV.TryParse(memory.Value.Span, out long svValue))
                {
                    value = svValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                break;
            case DicomVR.TM:
                // TODO return raw value
                if(_valueParser.TM.TryParse(memory.Value.Span, out var tmValue))
                {
                    value = tmValue.ToString("O");
                    return true;
                }

                break;
            case DicomVR.UC:
                return _valueParser.UC.TryParse(memory.Value.Span, out value);
            case DicomVR.UI:
                return _valueParser.UI.TryParse(memory.Value.Span, out value);
            case DicomVR.UL:
                if(_valueParser.UL.TryParse(memory.Value.Span, out uint ulValue))
                {
                    value = ulValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                break;
            case DicomVR.US:
                if(_valueParser.US.TryParse(memory.Value.Span, out ushort usValue))
                {
                    value = usValue.ToString();
                    return true;
                }

                break;
            case DicomVR.UT:
                return _valueParser.UT.TryParse(memory.Value.Span, encoding, out value);
            case DicomVR.UV:
                if(_valueParser.UV.TryParse(memory.Value.Span, out ulong uvValue))
                {
                    value = uvValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                break;
        }

        value = default;
        return false;
    }
}
