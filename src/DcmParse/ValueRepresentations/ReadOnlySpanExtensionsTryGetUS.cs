namespace DcmParse.ValueRepresentations;

internal static class ReadOnlySpanExtensionsTryGetUS
{
    public static bool TryGetUS(this ReadOnlySpan<byte> span, out ushort value)
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
