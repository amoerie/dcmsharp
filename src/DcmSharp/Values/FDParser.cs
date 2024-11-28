namespace DcmSharp.Values;

internal sealed class FDParser
{
    public bool TryParse(ReadOnlySpan<byte> span, out double value)
    {
        if (span.Length != 8)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToDouble(span);
        return true;
    }
}
