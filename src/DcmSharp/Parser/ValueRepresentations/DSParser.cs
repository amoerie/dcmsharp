using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class DSParser
{
    public bool TryParse(ReadOnlySpan<byte> span, out double value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        const int maxLength = 16;
        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimSpaces(span);
        Span<char> charSpan = stackalloc char[Math.Min(maxLength, trimmedSpan.Length)];
        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];
        return double.TryParse(charSpan, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    public bool TryParseString(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = Encoding.ASCII.GetString(DicomPadding.TrimSpaces(span));
        return true;
    }

    [SkipLocalsInit]
    public bool TryParseAll(ReadOnlySpan<byte> span, [NotNullWhen(true)] out double[]? values)
    {
        if (span.IsEmpty)
        {
            values = default;
            return false;
        }

        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimSpaces(span);

        char[]? sharedChars = null;
        Span<char> charSpan = trimmedSpan.Length < 255
            ? stackalloc char[trimmedSpan.Length]
            : ArrayPool<char>.Shared.Rent(trimmedSpan.Length);
        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];

        int numberOfValues = charSpan.Count('\\');
        Range[]? sharedRanges = null;
        Span<Range> ranges = numberOfValues < 16
            ? stackalloc Range[numberOfValues]
            : ArrayPool<Range>.Shared.Rent(numberOfValues);
        MemoryExtensions.Split(charSpan, ranges, '\\');

        values = new double[numberOfValues];

        bool allOk = true;
        for (int i = 0; i < ranges.Length; i++)
        {
            Range range = ranges[i];
            if (double.TryParse(charSpan[range], NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                values[i] = value;
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
