using DcmParse.ValueRepresentations;

namespace DcmParse;

public static class DicomDatasetExtensionsTryGetDateTime
{
    public static bool TryGetDate(this DicomDataset dataset, DicomTag tag, out DateTime value)
        => TryGetDate(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetDate(this DicomDataset dataset, ushort group, ushort element, out DateTime value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.DT:
                return memory.Value.Span.TryGetDT(out value);
        }

        value = default;
        return false;
    }
}
