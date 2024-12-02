using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class ISParser
{
    private const int MaxLength = 12;

    public bool TryParse<TNumber>(ReadOnlySpan<byte> span, out TNumber value) where TNumber: struct, INumber<TNumber>
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
        return TNumber.TryParse(charSpan, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);
    }

    [SkipLocalsInit]
    public bool TryParseAll<TNumber>(ReadOnlySpan<byte> span, out TNumber[] values) where TNumber: struct, INumber<TNumber>
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
            : ArrayPool<char>.Shared.Rent(trimmedSpan.Length);

        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];

        int numberOfValues = charSpan.Count('\\') + 1;
        Range[]? sharedRanges = null;
        Span<Range> ranges = numberOfValues < 16
            ? stackalloc Range[numberOfValues]
            : ArrayPool<Range>.Shared.Rent(numberOfValues);
        MemoryExtensions.Split(charSpan, ranges, '\\');

        values = new TNumber[numberOfValues];

        bool allOk = true;
        for (int i = 0; i < ranges.Length; i++)
        {
            Range range = ranges[i];
            if (TNumber.TryParse(charSpan[range], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out TNumber value))
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

}
