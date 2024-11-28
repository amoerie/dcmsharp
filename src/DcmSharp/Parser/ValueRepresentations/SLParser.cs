namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class SLParser
{
    public bool TryParse(ReadOnlySpan<byte> span, out int value)
    {
        if (span.Length != 4)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToInt32(span);
        return true;
    }
}
