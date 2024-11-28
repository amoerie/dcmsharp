using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class ASParser
{
    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
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
