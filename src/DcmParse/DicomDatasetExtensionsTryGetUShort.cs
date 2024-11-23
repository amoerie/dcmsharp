namespace DcmParse;

public static class DicomDatasetExtensionsTryGetUShort
{
    public static bool TryGetUShort(this DicomDataset dataset, DicomTag tag, out ushort value)
    {
        if (!dataset.TryGetRaw(tag, out ReadOnlyMemory<byte>? raw) || raw.Value.Length != sizeof(ushort))
        {
            value = default;
            return false;
        }

        value = BitConverter.ToUInt16(raw.Value.Span);
        return true;
    }
}
