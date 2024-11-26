using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmParse.Values;

internal sealed class UTParser
{
    public bool TryParse(ReadOnlySpan<byte> span, Encoding encoding, [NotNullWhen(true)] out string? value)
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
