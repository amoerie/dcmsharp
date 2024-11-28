namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class USParser
{
    public bool TryParse(ReadOnlySpan<byte> span, out ushort value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToUInt16(span);
        return true;
    }
}
