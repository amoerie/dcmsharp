using System.Diagnostics.CodeAnalysis;
using System.Text;
using DcmSharp.Parser;

namespace DcmSharp.Values;

internal sealed class UCParser
{
    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        // TODO apply encoding found in (0008,0005)
        value = Encoding.ASCII.GetString(DicomPadding.TrimEndSpaces(span));
        return true;
    }
}
