using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmParse.ValueRepresentations;

public static class ReadOnlySpanExtensionsTryGetCS
{
    public static bool TryGetCS(this ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
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
