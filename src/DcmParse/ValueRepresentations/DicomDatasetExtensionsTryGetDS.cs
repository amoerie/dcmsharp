namespace DcmParse.ValueRepresentations;

public static class DicomDatasetExtensionsTryGetDS
{
    public static bool TryGetDS(this DicomDataset dataset, DicomTag tag, out double value)
        => TryGetDS(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetDS(this DicomDataset dataset, ushort group, ushort element, out double value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? raw, out DicomVR? vr) || vr != DicomVR.DS)
        {
            value = default;
            return false;
        }

        return raw.Value.Span.TryGetDS(out value);
    }
}
