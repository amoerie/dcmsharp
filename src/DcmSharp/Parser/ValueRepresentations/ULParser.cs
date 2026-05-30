using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class ULParser
{
    private const int Length = 4;

    public bool TryParse(ReadOnlySpan<byte> span, out uint value)
    {
        if (span.Length != Length)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToUInt32(span);
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out long value)
    {
        if (!TryParse(span, out uint number))
        {
            value = default;
            return false;
        }

        value = number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out ulong value)
    {
        if (!TryParse(span, out uint number))
        {
            value = default;
            return false;
        }

        value = number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out float value)
    {
        if (!TryParse(span, out uint number))
        {
            value = default;
            return false;
        }

        value = number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out double value)
    {
        if (!TryParse(span, out uint number))
        {
            value = default;
            return false;
        }

        value = number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out decimal value)
    {
        if (!TryParse(span, out uint number))
        {
            value = default;
            return false;
        }

        value = number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (!TryParse(span, out uint number))
        {
            value = default;
            return false;
        }

        value = number.ToString(CultureInfo.InvariantCulture);
        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out uint[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new uint[span.Length / Length];
        for (int i = 0; i < values.Length; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToUInt32(span.Slice(offset, Length));
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

        values = new long[span.Length / Length];
        for (int i = 0; i < values.Length; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToUInt32(span.Slice(offset, Length));
        }

        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out ulong[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new ulong[span.Length / Length];
        for (int i = 0; i < values.Length; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToUInt32(span.Slice(offset, Length));
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

        values = new float[span.Length / Length];
        for (int i = 0; i < values.Length; i++)
        {
            int offset = i * Length;
            values[i] = BitConverter.ToUInt32(span.Slice(offset, Length));
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
            values[i] = BitConverter.ToUInt32(span.Slice(offset, Length));
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
            values[i] = BitConverter.ToUInt32(span.Slice(offset, Length));
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
                .ToUInt32(span.Slice(offset, Length))
                .ToString(CultureInfo.InvariantCulture);
        }

        return true;
    }
}
