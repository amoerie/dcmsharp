using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
        "yyyy/MM/dd"
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
        if (DateTime.TryParseExact(charSpan, _formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime parsedDate))
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
}
