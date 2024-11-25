using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmParse.ValueRepresentations;

public static class ReadOnlySpanExtensionsTryGetAS
{
    public static bool TryGetAS(this ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = Encoding.ASCII.GetString(span);
        return true;
    }
}
