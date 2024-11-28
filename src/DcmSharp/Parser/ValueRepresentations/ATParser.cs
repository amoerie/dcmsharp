namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class ATParser
{
    public bool TryParse(ReadOnlySpan<byte> span, out (ushort group, ushort element)? value)
    {
        if (span.Length != 4)
        {
            value = default;
            return false;
        }

        ushort tagGroup = BitConverter.ToUInt16(span[..2]);
        ushort tagElement = BitConverter.ToUInt16(span[2..]);

        value = (tagGroup, tagElement);
        return true;
    }
}
