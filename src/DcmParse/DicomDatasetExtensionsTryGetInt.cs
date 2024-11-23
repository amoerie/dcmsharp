namespace DcmParse;

public static class DicomDatasetExtensionsTryGetInt
{
    public static bool TryGetInt(this DicomDataset dataset, DicomTag tag, out int value)
    {
        if (!dataset.TryGetRaw(tag, out ReadOnlyMemory<byte>? raw) || raw.Value.Length != sizeof(int))
        {
            value = default;
            return false;
        }

        value = BitConverter.ToInt32(raw.Value.Span);
        return true;
    }

}
