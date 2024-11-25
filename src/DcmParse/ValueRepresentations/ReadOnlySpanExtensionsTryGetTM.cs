using System.Globalization;
using System.Text;

namespace DcmParse.ValueRepresentations;

internal static class ReadOnlySpanExtensionsTryGetTM
{
    private const int MaxLength = 16;

    private static readonly string[] _formats =
    [
        "HHmmss",
        "HH",
        "HHmm",
        "HHmmssf",
        "HHmmssff",
        "HHmmssfff",
        "HHmmssffff",
        "HHmmssfffff",
        "HHmmssffffff",
        "HHmmss.f",
        "HHmmss.ff",
        "HHmmss.fff",
        "HHmmss.ffff",
        "HHmmss.fffff",
        "HHmmss.ffffff",
        "HH.mm",
        "HH.mm.ss",
        "HH.mm.ss.f",
        "HH.mm.ss.ff",
        "HH.mm.ss.fff",
        "HH.mm.ss.ffff",
        "HH.mm.ss.fffff",
        "HH.mm.ss.ffffff",
        "HH:mm",
        "HH:mm:ss",
        "HH:mm:ss:f",
        "HH:mm:ss:ff",
        "HH:mm:ss:fff",
        "HH:mm:ss:ffff",
        "HH:mm:ss:fffff",
        "HH:mm:ss:ffffff",
        "HH:mm:ss.f",
        "HH:mm:ss.ff",
        "HH:mm:ss.fff",
        "HH:mm:ss.ffff",
        "HH:mm:ss.fffff",
        "HH:mm:ss.ffffff",
    ];

    public static bool TryGetTM(this ReadOnlySpan<byte> span, out TimeOnly value)
    {
        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimEndSpaces(span);
        Span<char> charSpan = stackalloc char[Math.Min(MaxLength, trimmedSpan.Length)];
        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];

        if (TimeOnly.TryParseExact(charSpan, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out TimeOnly parsedTime))
        {
            value = parsedTime;
            return true;
        }

        value = default;
        return false;
    }
}
