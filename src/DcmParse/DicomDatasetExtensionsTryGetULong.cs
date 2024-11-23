namespace DcmParse;

public static class DicomDatasetExtensionsTryGetULong
{
    public static bool TryGetLong(this DicomDataset dataset, DicomTag tag, out ulong value)
    {
        if (!dataset.TryGetRaw(tag, out ReadOnlyMemory<byte>? raw) || raw.Value.Length != sizeof(ulong))
        {
            value = default;
            return false;
        }

        value = BitConverter.ToUInt64(raw.Value.Span);
        return true;
    }
}
