using System.Globalization;
using System.Text;

namespace DcmParse.ValueRepresentations;

internal static class ReadOnlySpanExtensionsTryGetIS
{
    private const int MaxLength = 12;

    public static bool TryGetIS(this ReadOnlySpan<byte> span, out int value)
    {
        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimSpaces(span);
        Span<char> charSpan = stackalloc char[Math.Min(MaxLength, trimmedSpan.Length)];
        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];
        return int.TryParse(charSpan, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }
}
