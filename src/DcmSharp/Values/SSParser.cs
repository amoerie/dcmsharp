namespace DcmSharp.Values;

internal sealed class SSParser
{
    public bool TryParse(ReadOnlySpan<byte> span, out short value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToInt16(span);
        return true;
    }
}
