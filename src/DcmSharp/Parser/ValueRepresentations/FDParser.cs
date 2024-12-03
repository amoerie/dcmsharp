using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class FDParser
{
    private const int Length = 8;

    public bool TryParse(ReadOnlySpan<byte> span, out double value)
    {
        if (span.Length != Length)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToDouble(span);
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (!TryParse(span, out double number))
        {
            value = default;
            return false;
        }

        value = number.ToString(CultureInfo.InvariantCulture);
        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out double[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new double[Length];
        int numberOfValues = span.Length / Length;
        for (int i = 0; i < numberOfValues; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToDouble(span.Slice(offset, Length));
        }

        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out string[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new string[Length];
        int numberOfValues = span.Length / Length;
        for (int i = 0; i < numberOfValues; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToDouble(span.Slice(offset, Length)).ToString(CultureInfo.InvariantCulture);
        }

        return true;
    }
}
