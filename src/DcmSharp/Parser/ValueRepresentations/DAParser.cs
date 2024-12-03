using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class DAParser
{
    private const int MaxLength = 8;

    private static readonly string[] _formats =
    [
        "yyyyMMdd",
        "yyyy.MM.dd",
        "yyyy/MM/dd",
        "yyyy",
        "yyyyMM",
        "yyyy.MM",
    ];

    public bool TryParse(ReadOnlySpan<byte> span, out DateOnly value)
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

        if (DateOnly.TryParseExact(charSpan, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsedDate))
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
