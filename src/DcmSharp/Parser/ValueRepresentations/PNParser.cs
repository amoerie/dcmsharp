using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class PNParser
{
    [SkipLocalsInit]
    public bool TryParse(ReadOnlySpan<byte> span, Encoding encoding, out PersonName value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        char[] chars = ArrayPool<char>.Shared.Rent(span.Length);
        try
        {
            int charLength = encoding.GetChars(span, chars);
            ReadOnlySpan<char> charSpan = chars.AsSpan(0, charLength);

            value = Parse(charSpan);
            return true;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(chars);
        }
    }

    [SkipLocalsInit]
    private static PersonName Parse(ReadOnlySpan<char> span)
    {
        string? familyName = null,
            givenName = null,
            middleName = null,
            namePrefix = null,
            nameSuffix = null,
            ideographicFamilyName = null,
            ideographicGivenName = null,
            phoneticFamilyName = null,
            phoneticGivenName = null;

        // Split into component groups (alphabetic, ideographic, phonetic) using '=' delimiter
        Span<Range> groupRanges = stackalloc Range[3];
        int numberOfGroupRanges = span.Split(groupRanges, '=');

        // Process the first (alphabetic) component group
        if (numberOfGroupRanges > 0)
        {
            Range groupRange = groupRanges[0];
            ReadOnlySpan<char> group = span[groupRange];
            Span<Range> ranges = stackalloc Range[5];

            // Split components using '^' delimiter
            int numberOfRanges = group.Split(ranges, '^');

            // Assign components based on position
            if (numberOfRanges > 0)
            {
                var range = ranges[0];
                var rangeSpan = group[range];
                var trimmedRangeSpan = DicomPadding.TrimEndSpaces(rangeSpan);
                familyName = new string(trimmedRangeSpan);
            }
            if (numberOfRanges > 1)
            {
                var range = ranges[1];
                var rangeSpan = group[range];
                var trimmedRangeSpan = DicomPadding.TrimEndSpaces(rangeSpan);
                givenName = new string(trimmedRangeSpan);
            }
            if (numberOfRanges > 2)
            {
                var range = ranges[2];
                var rangeSpan = group[range];
                var trimmedRangeSpan = DicomPadding.TrimEndSpaces(rangeSpan);
                middleName = new string(trimmedRangeSpan);
            }
            if (numberOfRanges > 3)
            {
                var range = ranges[3];
                var rangeSpan = group[range];
                var trimmedRangeSpan = DicomPadding.TrimEndSpaces(rangeSpan);
                namePrefix = new string(trimmedRangeSpan);
            }
            if (numberOfRanges > 4)
            {
                var range = ranges[4];
                var rangeSpan = group[range];
                var trimmedRangeSpan = DicomPadding.TrimEndSpaces(rangeSpan);
                nameSuffix = new string(trimmedRangeSpan);
            }
        }

        // Process the second (ideographic) component group
        if (numberOfGroupRanges > 1)
        {
            Range groupRange = groupRanges[1];
            ReadOnlySpan<char> group = span[groupRange];
            Span<Range> ranges = stackalloc Range[2];

            // Split components using '^' delimiter
            int numberOfRanges = group.Split(ranges, '^');

            // Assign components based on position
            if (numberOfRanges > 0)
            {
                var range = ranges[0];
                var rangeSpan = group[range];
                var trimmedRangeSpan = DicomPadding.TrimEndSpaces(rangeSpan);
                ideographicFamilyName = new string(trimmedRangeSpan);
            }
            if (numberOfRanges > 1)
            {
                var range = ranges[1];
                var rangeSpan = group[range];
                var trimmedRangeSpan = DicomPadding.TrimEndSpaces(rangeSpan);
                ideographicGivenName = new string(trimmedRangeSpan);
            }
        }

        // Process the third (phonetic) component group
        if (numberOfGroupRanges > 2)
        {
            Range groupRange = groupRanges[2];
            ReadOnlySpan<char> group = span[groupRange];
            Span<Range> ranges = stackalloc Range[2];

            // Split components using '^' delimiter
            int numberOfRanges = group.Split(ranges, '^');

            // Assign components based on position
            if (numberOfRanges > 0)
            {
                var range = ranges[0];
                var rangeSpan = group[range];
                var trimmedRangeSpan = DicomPadding.TrimEndSpaces(rangeSpan);
                phoneticFamilyName = new string(trimmedRangeSpan);
            }
            if (numberOfRanges > 1)
            {
                var range = ranges[1];
                var rangeSpan = group[range];
                var trimmedRangeSpan = DicomPadding.TrimEndSpaces(rangeSpan);
                phoneticGivenName = new string(trimmedRangeSpan);
            }
        }

        return new PersonName(
            familyName,
            givenName,
            middleName,
            namePrefix,
            nameSuffix,
            ideographicFamilyName,
            ideographicGivenName,
            phoneticFamilyName,
            phoneticGivenName
        );
    }

    public bool TryParse(
        ReadOnlySpan<byte> span,
        Encoding encoding,
        [NotNullWhen(true)] out string? value
    )
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = encoding.GetString(DicomPadding.TrimEndSpaces(span));
        return true;
    }

    [SkipLocalsInit]
    public bool TryParseAll(ReadOnlySpan<byte> span, Encoding encoding, out PersonName[] values)
    {
        if (span.IsEmpty)
        {
            values = [];
            return false;
        }

        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimEndSpaces(span);

        char[]? sharedChars = null;
        Span<char> charSpan =
            trimmedSpan.Length < 255
                ? stackalloc char[trimmedSpan.Length]
                : sharedChars = ArrayPool<char>.Shared.Rent(trimmedSpan.Length);

        int written = encoding.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];

        int numberOfValues = charSpan.Count('\\') + 1;
        Range[]? sharedRanges = null;
        Span<Range> ranges =
            numberOfValues < 16
                ? stackalloc Range[numberOfValues]
                : sharedRanges = ArrayPool<Range>.Shared.Rent(numberOfValues);
        MemoryExtensions.Split(charSpan, ranges, '\\');

        values = new PersonName[numberOfValues];

        for (int i = 0; i < ranges.Length; i++)
        {
            Range range = ranges[i];
            ReadOnlySpan<char> valueSpan = charSpan[range];
            values[i] = Parse(valueSpan);
        }

        if (sharedChars is not null)
        {
            ArrayPool<char>.Shared.Return(sharedChars);
        }
        if (sharedRanges is not null)
        {
            ArrayPool<Range>.Shared.Return(sharedRanges);
        }

        return true;
    }

    [SkipLocalsInit]
    public bool TryParseAll(ReadOnlySpan<byte> span, Encoding encoding, out string[] values)
    {
        if (span.IsEmpty)
        {
            values = [];
            return false;
        }

        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimEndSpaces(span);

        char[]? sharedChars = null;
        Span<char> charSpan =
            trimmedSpan.Length < 255
                ? stackalloc char[trimmedSpan.Length]
                : sharedChars = ArrayPool<char>.Shared.Rent(trimmedSpan.Length);

        int written = encoding.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];

        int numberOfValues = charSpan.Count('\\') + 1;
        Range[]? sharedRanges = null;
        Span<Range> ranges =
            numberOfValues < 16
                ? stackalloc Range[numberOfValues]
                : sharedRanges = ArrayPool<Range>.Shared.Rent(numberOfValues);
        MemoryExtensions.Split(charSpan, ranges, '\\');

        values = new string[numberOfValues];

        for (int i = 0; i < ranges.Length; i++)
        {
            Range range = ranges[i];
            values[i] = new string(charSpan[range]);
        }

        if (sharedChars is not null)
        {
            ArrayPool<char>.Shared.Return(sharedChars);
        }
        if (sharedRanges is not null)
        {
            ArrayPool<Range>.Shared.Return(sharedRanges);
        }

        return true;
    }
}
