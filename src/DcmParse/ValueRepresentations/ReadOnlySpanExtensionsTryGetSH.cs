using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmParse.ValueRepresentations;

internal static class ReadOnlySpanExtensionsTryGetSH
{
    public static bool TryGetSH(this ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
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
