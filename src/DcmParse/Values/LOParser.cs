using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmParse.Values;

internal sealed class LOParser
{
    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        // TODO apply encoding found in (0008,0005)
        value = Encoding.ASCII.GetString(DicomPadding.TrimSpaces(span));
        return true;
    }
}
