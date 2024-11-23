namespace DcmParse;

public static class DicomDatasetExtensionsTryGetLong
{
    public static bool TryGetLong(this DicomDataset dataset, DicomTag tag, out long value)
    {
        if (!dataset.TryGetRaw(tag, out ReadOnlyMemory<byte>? raw) || raw.Value.Length != sizeof(long))
        {
            value = default;
            return false;
        }

        value = BitConverter.ToInt64(raw.Value.Span);
        return true;
    }
}
