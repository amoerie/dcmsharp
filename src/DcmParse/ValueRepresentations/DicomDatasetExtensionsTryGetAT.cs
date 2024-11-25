namespace DcmParse.ValueRepresentations;

public static class DicomDatasetExtensionsTryGetAT
{
    public static bool TryGetAT(this DicomDataset dataset, DicomTag tag, out (ushort group, ushort element)? value)
        => TryGetAT(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetAT(this DicomDataset dataset, ushort group, ushort element, out (ushort group, ushort element)? value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? raw, out DicomVR? vr) || vr != DicomVR.AT)
        {
            value = default;
            return false;
        }

        if (raw.Value.Length != sizeof(ushort) * 2)
        {
            value = default;
            return false;
        }

        ReadOnlySpan<byte> span = raw.Value.Span;
        ushort tagGroup = BitConverter.ToUInt16(span[..2]);
        ushort tagElement = BitConverter.ToUInt16(span[2..]);

        value = (tagGroup, tagElement);
        return true;
    }
}
