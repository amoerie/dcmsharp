using System.Diagnostics.CodeAnalysis;
using System.Text;
using DcmSharp.Parser;

namespace DcmSharp.Values;

internal sealed class LTParser
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
