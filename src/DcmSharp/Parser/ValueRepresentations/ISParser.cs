using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class ISParser
{
    private const int MaxLength = 12;

    public bool TryParse(ReadOnlySpan<byte> span, out int value)
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
        return int.TryParse(charSpan, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
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
