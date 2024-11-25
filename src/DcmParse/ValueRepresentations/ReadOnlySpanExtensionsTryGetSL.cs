namespace DcmParse.ValueRepresentations;

internal static class ReadOnlySpanExtensionsTryGetSL
{
    public static bool TryGetSL(this ReadOnlySpan<byte> span, out int value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToInt32(span);
        return true;
    }
}
