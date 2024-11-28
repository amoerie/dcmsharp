namespace DcmSharp.Values;

internal sealed class FLParser
{
    public bool TryParse(ReadOnlySpan<byte> span, out float value)
    {
        if (span.Length != 4)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToSingle(span);
        return true;
    }
}
