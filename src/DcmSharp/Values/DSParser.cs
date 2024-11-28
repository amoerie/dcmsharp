using System.Globalization;
using System.Text;
using DcmSharp.Parser;

namespace DcmSharp.Values;

internal sealed class DSParser
{
    private const int MaxLength = 16;

    public bool TryParse(ReadOnlySpan<byte> span, out double value)
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
        return double.TryParse(charSpan, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
