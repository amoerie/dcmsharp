using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetDoubles(DicomTag tag, [NotNullWhen(true)] out double[]? values) =>
        TryGetDoubles(tag.Group, tag.Element, out values);

    public bool TryGetDoubles(ushort group, ushort element, [NotNullWhen(true)] out double[]? values)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.DS:
                return _valueParser.DS.TryParseAll(memory.Value.Span, out values);
            case DicomVR.FD:
                // TODO
                // return _valueParser.FD.TryParse(memory.Value.Span, out value);
            case DicomVR.FL:
                // if (_valueParser.FL.TryParse(memory.Value.Span, out float flValue))
                // {
                //     value = flValue;
                //     return true;
                // }

                break;
            case DicomVR.IS:
                // if (_valueParser.IS.TryParse(memory.Value.Span, out int isValue))
                // {
                //     value = isValue;
                //     return true;
                // }

                break;
            case DicomVR.SL:
                // if (_valueParser.SL.TryParse(memory.Value.Span, out int slValue))
                // {
                //     value = slValue;
                //     return true;
                // }

                break;
            case DicomVR.SS:
                // if (_valueParser.SS.TryParse(memory.Value.Span, out short ssValue))
                // {
                //     value = ssValue;
                //     return true;
                // }

                break;
            case DicomVR.SV:
                // if (_valueParser.SV.TryParse(memory.Value.Span, out long svValue))
                // {
                //     value = svValue;
                //     return true;
                // }

                break;
            case DicomVR.UL:
                // if (_valueParser.UL.TryParse(memory.Value.Span, out uint uintValue))
                // {
                //     value = uintValue;
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
                // if (_valueParser.UV.TryParse(memory.Value.Span, out ulong uvValue))
                // {
                //     value = uvValue;
                //     return true;
                // }

                break;
        }

        values = default;
        return false;
    }
}
