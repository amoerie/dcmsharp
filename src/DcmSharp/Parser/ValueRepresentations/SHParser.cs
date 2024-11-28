using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class SHParser
{
    public bool TryParse(ReadOnlySpan<byte> span, Encoding encoding, [NotNullWhen(true)] out string? value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = encoding.GetString(DicomPadding.TrimSpaces(span));
        return true;
    }
}
