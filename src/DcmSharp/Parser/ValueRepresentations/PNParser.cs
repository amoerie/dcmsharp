using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class PNParser
{
    [SkipLocalsInit]
    public bool TryParse(ReadOnlySpan<byte> span, Encoding encoding, out DicomPersonName value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        char[] chars = ArrayPool<char>.Shared.Rent(span.Length);
        try
        {
            string?
                familyName = null,
                givenName = null,
                middleName = null,
                namePrefix = null,
                nameSuffix = null,
                ideographicFamilyName = null,
                ideographicGivenName = null,
                phoneticFamilyName = null,
                phoneticGivenName = null;

            int charLength = encoding.GetChars(span, chars);
            ReadOnlySpan<char> charSpan = chars.AsSpan(0, charLength);

            // Split into component groups (alphabetic, ideographic, phonetic) using '=' delimiter
            Span<Range> groupRanges = stackalloc Range[3];
            int numberOfGroupRanges = charSpan.Split(groupRanges, '=');

            // Process the first (alphabetic) component group
            if (numberOfGroupRanges > 0)
            {
                Range groupRange = groupRanges[0];
                ReadOnlySpan<char> group = charSpan[groupRange];
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
                ReadOnlySpan<char> group = charSpan[groupRange];
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
                ReadOnlySpan<char> group = charSpan[groupRange];
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

            value = new DicomPersonName(familyName, givenName, middleName, namePrefix, nameSuffix,
                ideographicFamilyName, ideographicGivenName, phoneticFamilyName, phoneticGivenName);
            return true;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(chars);
        }
    }

    public bool TryParseString(ReadOnlySpan<byte> span, Encoding encoding, [NotNullWhen(true)] out string? value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = encoding.GetString(DicomPadding.TrimEndSpaces(span));
        return true;
    }

}
