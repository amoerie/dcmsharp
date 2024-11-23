namespace DcmParse;

public static class DicomDatasetExtensionsTryGetShort
{
    public static bool TryGetShort(this DicomDataset dataset, DicomTag tag, out short value)
    {
        if (!dataset.TryGetRaw(tag, out ReadOnlyMemory<byte>? raw) || raw.Value.Length != sizeof(short))
        {
            value = default;
            return false;
        }

        value = BitConverter.ToInt16(raw.Value.Span);
        return true;
    }
}
