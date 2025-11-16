using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class DSParser
{
    private const int MaxLength = 12;

    public bool TryParse(ReadOnlySpan<byte> span, out decimal value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimSpaces(span);
        Span<char> charSpan = stackalloc char[Math.Min(MaxLength, trimmedSpan.Length)];
        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];
        return decimal.TryParse(charSpan, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    public bool TryParse(ReadOnlySpan<byte> span, out double value)
    {
        if (!TryParse(span, out decimal number))
        {
            value = default;
            return false;
        }

        value = (double) number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, out float value)
    {
        if (!TryParse(span, out decimal number))
        {
            value = default;
            return false;
        }

        value = (float) number;
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = Encoding.ASCII.GetString(DicomPadding.TrimSpaces(span));
        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out decimal[] values)
    {
        return TryParseAll(span, static x => x, out values);
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out double[] values)
    {
        return TryParseAll(span, static x => (double) x, out values);
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out float[] values)
    {
        return TryParseAll(span, static x => (float) x, out values);
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out string[] values)
    {
        return TryParseAll(span, static x => x.ToString(CultureInfo.InvariantCulture), out values);
    }

    [SkipLocalsInit]
    private static bool TryParseAll<T>(ReadOnlySpan<byte> span, Func<decimal, T> converter, out T[] values)
    {
        if (span.IsEmpty)
        {
            values = [];
            return false;
        }

        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimSpaces(span);

        char[]? sharedChars = null;
        Span<char> charSpan = trimmedSpan.Length < 255
            ? stackalloc char[trimmedSpan.Length]
            : sharedChars = ArrayPool<char>.Shared.Rent(trimmedSpan.Length);

        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];

        int numberOfValues = charSpan.Count('\\') + 1;
        Range[]? sharedRanges = null;
        Span<Range> ranges = numberOfValues < 16
            ? stackalloc Range[numberOfValues]
            : sharedRanges = ArrayPool<Range>.Shared.Rent(numberOfValues);
        MemoryExtensions.Split(charSpan, ranges, '\\');

        values = new T[numberOfValues];

        bool allOk = true;
        for (int i = 0; i < ranges.Length; i++)
        {
            Range range = ranges[i];
            if (decimal.TryParse(charSpan[range], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out decimal value))
            {
                values[i] = converter(value);
            }
            else
            {
                allOk = false;
                break;
            }
        }

        if (sharedChars is not null)
        {
            ArrayPool<char>.Shared.Return(sharedChars);
        }
        if (sharedRanges is not null)
        {
            ArrayPool<Range>.Shared.Return(sharedRanges);
        }

        return allOk;
    }

}
