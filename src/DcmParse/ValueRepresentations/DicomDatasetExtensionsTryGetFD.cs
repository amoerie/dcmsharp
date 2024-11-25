namespace DcmParse.ValueRepresentations;

public static class DicomDatasetExtensionsTryGetFD
{
    public static bool TryGetFD(this DicomDataset dataset, DicomTag tag, out double value)
        => TryGetFD(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetFD(this DicomDataset dataset, ushort group, ushort element, out double value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? raw, out DicomVR? vr) || vr != DicomVR.FD)
        {
            value = default;
            return false;
        }

        value = BitConverter.ToDouble(raw.Value.Span);
        return true;
    }
}
