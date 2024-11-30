using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetValue(DicomTag tag, [NotNullWhen(true)] out ReadOnlyMemory<byte>? value, [NotNullWhen(true)] out DicomVR? vr) =>
        TryGetValue(tag.Group, tag.Element, out value, out vr);

    public bool TryGetValue(ushort group, ushort element, [NotNullWhen(true)] out ReadOnlyMemory<byte>? value, [NotNullWhen(true)] out DicomVR? vr)
    {
        if(!_items.TryGetValue((uint)group << 16 | element, out var item))
        {
            value = default;
            vr = default;
            return false;
        }

        if (item.Content.Value is { } dicomValue)
        {
            value = dicomValue;
            vr = item.VR;
            return true;
        }

        value = default;
        vr = default;
        return false;
    }

}
