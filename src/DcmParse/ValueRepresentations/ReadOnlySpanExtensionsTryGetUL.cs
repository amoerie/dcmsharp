using System.Globalization;
using System.Text;

namespace DcmParse.ValueRepresentations;

internal static class ReadOnlySpanExtensionsTryGetUL
{
    public static bool TryGetUL(this ReadOnlySpan<byte> span, out uint value)
    {
        value = BitConverter.ToUInt32(span);
        return true;
    }
}
