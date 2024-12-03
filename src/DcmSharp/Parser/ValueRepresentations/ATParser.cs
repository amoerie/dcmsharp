using System.Diagnostics.CodeAnalysis;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class ATParser
{
    public bool TryParse(ReadOnlySpan<byte> span, out ushort group, out ushort element)
    {
        if (span.Length != 4)
        {
            group = default;
            element = default;
            return false;
        }

        group = BitConverter.ToUInt16(span[..2]);
        element = BitConverter.ToUInt16(span[2..]);
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (span.Length != 4)
        {
            value = default;
            return false;
        }

        value = $"{span[1]:X2}{span[0]:X2}{span[3]:X2}{span[2]:X2}";
        return true;
    }
}
