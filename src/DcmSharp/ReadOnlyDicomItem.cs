using System.Runtime.InteropServices;

namespace DcmSharp;

[StructLayout(LayoutKind.Auto)]
public readonly record struct ReadOnlyDicomItem(
    ushort Group,
    ushort Element,
    DicomVR VR,
    ReadOnlyDicomItemContent Content)
{
    public override string ToString()
    {
        return DicomTagsIndex.TryLookup(Group, Element, out var dicomTag)
            ? $"(0x{Group:x4},0x{Element:x4}) {VR} {dicomTag.Description}"
            : $"(0x{Group:x4},0x{Element:x4}) {VR}";
    }
}
