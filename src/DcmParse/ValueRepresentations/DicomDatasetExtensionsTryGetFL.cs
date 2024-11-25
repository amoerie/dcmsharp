namespace DcmParse.ValueRepresentations;

public static class DicomDatasetExtensionsTryGetFL
{
    public static bool TryGetFL(this DicomDataset dataset, DicomTag tag, out float value)
        => TryGetFL(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetFL(this DicomDataset dataset, ushort group, ushort element, out float value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? raw, out DicomVR? vr) || vr != DicomVR.FL)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToSingle(raw.Value.Span);
        return true;
    }
}
