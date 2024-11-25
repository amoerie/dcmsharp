using System.Globalization;
using System.Text;

namespace DcmParse.ValueRepresentations;

internal static class ReadOnlySpanExtensionsTryGetSS
{
    public static bool TryGetSS(this ReadOnlySpan<byte> span, out short value)
    {
        value = BitConverter.ToInt16(span);
        return true;
    }
}
