namespace DcmParse;

public static class DicomDatasetExtensionsTryGetDouble
{
    public static bool TryGetDouble(this DicomDataset dataset, DicomTag tag, out double value)
        => TryGetDouble(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetDouble(this DicomDataset dataset, ushort group, ushort element, out double value)
    {
        if (!dataset.TryGetRaw(group, element, out ReadOnlyMemory<byte>? raw) || raw.Value.Length != sizeof(double))
        {
            value = default;
            return false;
        }

        value = BitConverter.ToDouble(raw.Value.Span);
        return true;
    }

}
