namespace DcmParse.ValueRepresentations;

internal static class ReadOnlySpanExtensionsTryGetUL
{
    public static bool TryGetUL(this ReadOnlySpan<byte> span, out uint value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToUInt32(span);
        return true;
    }
}
