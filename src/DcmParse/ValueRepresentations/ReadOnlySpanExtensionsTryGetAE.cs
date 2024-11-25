using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmParse.ValueRepresentations;

internal static class ReadOnlySpanExtensionsTryGetAE
{
    public static bool TryGetAE(this ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        value = Encoding.ASCII.GetString(DicomPadding.TrimEndSpaces(span));
        return true;
    }
}
