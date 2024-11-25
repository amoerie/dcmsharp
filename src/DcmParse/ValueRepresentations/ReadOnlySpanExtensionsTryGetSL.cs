using System.Globalization;
using System.Text;

namespace DcmParse.ValueRepresentations;

internal static class ReadOnlySpanExtensionsTryGetSL
{
    public static bool TryGetSL(this ReadOnlySpan<byte> span, out int value)
    {
        value = BitConverter.ToInt32(span);
        return true;
    }
}
