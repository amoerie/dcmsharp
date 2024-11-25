using DcmParse.ValueRepresentations;

namespace DcmParse;

public static class DicomDatasetExtensionsTryGetTime
{
    public static bool TryGetTime(this DicomDataset dataset, DicomTag tag, out TimeOnly value)
        => TryGetTime(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetTime(this DicomDataset dataset, ushort group, ushort element, out TimeOnly value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.TM:
                return memory.Value.Span.TryGetTM(out value);
        }

        value = default;
        return false;
    }
}
