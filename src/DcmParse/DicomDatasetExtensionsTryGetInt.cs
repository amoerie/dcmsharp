using DcmParse.ValueRepresentations;

namespace DcmParse;

public static class DicomDatasetExtensionsTryGetInt
{
    public static bool TryGetInt(this DicomDataset dataset, DicomTag tag, out int value) =>
        TryGetInt(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetInt(this DicomDataset dataset, ushort group, ushort element, out int value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.IS:
                return memory.Value.Span.TryGetIS(out value);
            case DicomVR.SS:
                if (memory.Value.Span.TryGetSS(out short shortValue))
                {
                    value = shortValue;
                    return true;
                }

                break;
            case DicomVR.US:
                if (memory.Value.Span.TryGetUS(out ushort ushortValue))
                {
                    value = ushortValue;
                    return true;
                }

                break;
            case DicomVR.UL:
                if (memory.Value.Span.TryGetUL(out uint uintValue) && uintValue <= int.MaxValue)
                {
                    value = (int)uintValue;
                    return true;
                }

                break;
        }

        value = default;
        return false;
    }
}
