using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class DTParser
{
    private const int MaxLength = 26;

    private static readonly string[] _formats =
    [
        "yyyyMMddHHmmss",
        "yyyyMMddHHmmsszzz",
        "yyyyMMddHHmmsszz",
        "yyyyMMddHHmmssz",
        "yyyyMMddHHmmss.ffffff",
        "yyyyMMddHHmmss.fffff",
        "yyyyMMddHHmmss.ffff",
        "yyyyMMddHHmmss.fff",
        "yyyyMMddHHmmss.ff",
        "yyyyMMddHHmmss.f",
        "yyyyMMddHHmm",
        "yyyyMMddHH",
        "yyyyMMdd",
        "yyyyMM",
        "yyyy",
        "yyyyMMddHHmmss.ffffffzzz",
        "yyyyMMddHHmmss.fffffzzz",
        "yyyyMMddHHmmss.ffffzzz",
        "yyyyMMddHHmmss.fffzzz",
        "yyyyMMddHHmmss.ffzzz",
        "yyyyMMddHHmmss.fzzz",
        "yyyyMMddHHmmzzz",
        "yyyyMMddHHzzz",
        "yyyy.MM.dd",
        "yyyy/MM/dd",
    ];

    public bool TryParse(ReadOnlySpan<byte> span, out DateTime value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimEndSpaces(span);
        Span<char> charSpan = stackalloc char[Math.Min(MaxLength, trimmedSpan.Length)];
        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];

        // TODO respect time zone specified in (0008,0201)
        if (
            DateTime.TryParseExact(
                charSpan,
                _formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal,
                out DateTime parsedDate
            )
        )
        {
            value = parsedDate;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = Encoding.ASCII.GetString(DicomPadding.TrimEndSpaces(span));
        return true;
    }

    [SkipLocalsInit]
    public bool TryParseAll(ReadOnlySpan<byte> span, out DateTime[] values)
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

        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];

        int numberOfValues = charSpan.Count('\\') + 1;
        Range[]? sharedRanges = null;
        Span<Range> ranges =
            numberOfValues < 16
                ? stackalloc Range[numberOfValues]
                : sharedRanges = ArrayPool<Range>.Shared.Rent(numberOfValues);
        MemoryExtensions.Split(charSpan, ranges, '\\');

        values = new DateTime[numberOfValues];

        bool allOk = true;
        for (int i = 0; i < ranges.Length; i++)
        {
            Range range = ranges[i];

            if (
                !DateTime.TryParseExact(
                    charSpan[range],
                    _formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsedDate
                )
            )
            {
                allOk = false;
                break;
            }

            values[i] = parsedDate;
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

    [SkipLocalsInit]
    public bool TryParseAll(ReadOnlySpan<byte> span, out string[] values)
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

        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
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
