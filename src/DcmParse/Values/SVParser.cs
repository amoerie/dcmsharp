using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmParse.Values;

internal sealed class SVParser
{
    public bool TryParse(ReadOnlySpan<byte> span, out long value)
    {
        if (span.Length != 8)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToInt64(span);
        return true;
    }
}
