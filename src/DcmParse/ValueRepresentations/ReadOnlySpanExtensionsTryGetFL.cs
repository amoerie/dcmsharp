namespace DcmParse.ValueRepresentations;

public static class ReadOnlySpanExtensionsTryGetFL
{
    public static bool TryGetFL(this ReadOnlySpan<byte> span, out float value)
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
