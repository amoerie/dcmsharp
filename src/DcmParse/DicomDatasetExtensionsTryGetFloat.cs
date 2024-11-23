namespace DcmParse;

public static class DicomDatasetExtensionsTryGetFloat
{
    public static bool TryGetFloat(this DicomDataset dataset, DicomTag tag, out float value)
        => TryGetFloat(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetFloat(this DicomDataset dataset, ushort group, ushort element, out float value)
    {
        if (!dataset.TryGetRaw(group, element, out ReadOnlyMemory<byte>? raw) || raw.Value.Length != sizeof(float))
        {
            value = default;
            return false;
        }

        value = BitConverter.ToSingle(raw.Value.Span);
        return true;
    }

}
