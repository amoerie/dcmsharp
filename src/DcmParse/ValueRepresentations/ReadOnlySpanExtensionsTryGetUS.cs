using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace DcmParse.ValueRepresentations;

internal static class ReadOnlySpanExtensionsTryGetUS
{
    public static bool TryGetUS(this ReadOnlySpan<byte> span, out ushort value)
    {
        value = BitConverter.ToUInt16(span);
        return true;
    }
}
