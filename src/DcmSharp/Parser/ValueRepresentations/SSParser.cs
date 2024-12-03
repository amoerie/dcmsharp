using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class SSParser
{
    private const int Length = 2;

    public bool TryParse(ReadOnlySpan<byte> span, out short value)
    {
        if (span.Length != Length)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToInt16(span);
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out int value)
    {
        if (!TryParse(span, out short number))
        {
            value = default;
            return false;
        }

        value = number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out long value)
    {
        if (!TryParse(span, out short number))
        {
            value = default;
            return false;
        }

        value = number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out float value)
    {
        if (!TryParse(span, out short number))
        {
            value = default;
            return false;
        }

        value = number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out double value)
    {
        if (!TryParse(span, out short number))
        {
            value = default;
            return false;
        }

        value = number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out decimal value)
    {
        if (!TryParse(span, out short number))
        {
            value = default;
            return false;
        }

        value = number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (!TryParse(span, out short number))
        {
            value = default;
            return false;
        }

        value = number.ToString(CultureInfo.InvariantCulture);
        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out short[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new short[Length];
        int numberOfValues = span.Length / Length;
        for (int i = 0; i < numberOfValues; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToInt16(span.Slice(offset, Length));
        }

        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out int[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new int[Length];
        int numberOfValues = span.Length / Length;
        for (int i = 0; i < numberOfValues; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToInt16(span.Slice(offset, Length));
        }

        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out long[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new long[Length];
        int numberOfValues = span.Length / Length;
        for (int i = 0; i < numberOfValues; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToInt16(span.Slice(offset, Length));
        }

        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out float[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new float[Length];
        int numberOfValues = span.Length / Length;
        for (int i = 0; i < numberOfValues; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToInt16(span.Slice(offset, Length));
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

        values = new double[Length];
        int numberOfValues = span.Length / Length;
        for (int i = 0; i < numberOfValues; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToInt16(span.Slice(offset, Length));
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

        values = new decimal[Length];
        int numberOfValues = span.Length / Length;
        for (int i = 0; i < numberOfValues; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToInt16(span.Slice(offset, Length));
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
            values[i] = BitConverter.ToInt16(span.Slice(offset, Length)).ToString(CultureInfo.InvariantCulture);
        }

        return true;
    }
}
