using DcmParse.ValueRepresentations;

namespace DcmParse;

public static class DicomDatasetExtensionsTryGetDate
{
    public static bool TryGetDate(this DicomDataset dataset, DicomTag tag, out DateOnly value)
        => TryGetDate(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetDate(this DicomDataset dataset, ushort group, ushort element, out DateOnly value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.DA:
                return memory.Value.Span.TryGetDA(out value);
        }

        value = default;
        return false;
    }
}
