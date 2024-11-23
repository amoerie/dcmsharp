namespace DcmParse;

public static class DicomDatasetExtensionsTryGetUInt
{
    public static bool TryGetUInt(this DicomDataset dataset, DicomTag tag, out uint value)
    {
        if (!dataset.TryGetRaw(tag, out ReadOnlyMemory<byte>? raw) || raw.Value.Length != sizeof(uint))
        {
            value = default;
            return false;
        }

        value = BitConverter.ToUInt32(raw.Value.Span);
        return true;
    }

}
