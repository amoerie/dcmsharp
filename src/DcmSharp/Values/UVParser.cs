namespace DcmSharp.Values;

internal sealed class UVParser
{
    public bool TryParse(ReadOnlySpan<byte> span, out ulong value)
    {
        if (span.Length != 8)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToUInt64(span);
        return true;
    }
}
