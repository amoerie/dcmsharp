using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class FLParser
{
    private const int Length = 4;

    public bool TryParse(ReadOnlySpan<byte> span, out float value)
    {
        if (span.Length != Length)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToSingle(span);
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out double value)
    {
        if (!TryParse(span, out float number))
        {
            value = default;
            return false;
        }

        value = number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out decimal value)
    {
        if (!TryParse(span, out float number))
        {
            value = default;
            return false;
        }

        // We allow an explicit cast here because there is no "loss of precision" going from float to decimal
        // C# does not provide an implicit cast here because the conversion isn't exactly 1 to 1
        value = (decimal)number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (!TryParse(span, out float number))
        {
            value = default;
            return false;
        }

        value = number.ToString(CultureInfo.InvariantCulture);
        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out float[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new float[span.Length / Length];
        for (int i = 0; i < values.Length; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToSingle(span.Slice(offset, Length));
        }

        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out double[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new double[span.Length / Length];
        for (int i = 0; i < values.Length; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToSingle(span.Slice(offset, Length));
        }

        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out decimal[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new decimal[span.Length / Length];
        for (int i = 0; i < values.Length; i++)
        {
            int offset = i * Length;
            values[i] = (decimal)BitConverter.ToSingle(span.Slice(offset, Length));
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

        values = new string[span.Length / Length];
        for (int i = 0; i < values.Length; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter
                .ToSingle(span.Slice(offset, Length))
                .ToString(CultureInfo.InvariantCulture);
        }

        return true;
    }
}
