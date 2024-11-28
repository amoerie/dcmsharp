namespace DcmSharp.Values;

internal sealed class ULParser
{
    public bool TryParse(ReadOnlySpan<byte> span, out uint value)
    {
        if (span.Length != 4)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToUInt32(span);
        return true;
    }
}
