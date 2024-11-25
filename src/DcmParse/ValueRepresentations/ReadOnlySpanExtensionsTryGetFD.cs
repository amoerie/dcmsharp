namespace DcmParse.ValueRepresentations;

public static class ReadOnlySpanExtensionsTryGetFD
{
    public static bool TryGetFD(this ReadOnlySpan<byte> span, out double value)
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
