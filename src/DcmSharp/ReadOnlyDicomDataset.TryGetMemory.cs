using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetMemory(
        DicomTag tag,
        [NotNullWhen(true)] out ReadOnlyMemory<byte>? value,
        [NotNullWhen(true)] out DicomVR? vr
    ) => TryGetMemory(tag.Group, tag.Element, out value, out vr);

    public bool TryGetMemory(
        ushort group,
        ushort element,
        [NotNullWhen(true)] out ReadOnlyMemory<byte>? value,
        [NotNullWhen(true)] out DicomVR? vr
    )
    {
        if (!_items.TryGetValue((uint)group << 16 | element, out var item))
        {
            value = default;
            vr = default;
            return false;
        }

        if (item.Content.Memory is { } memory)
        {
            value = memory;
            vr = item.VR;
            return true;
        }

        value = default;
        vr = default;
        return false;
    }
}
