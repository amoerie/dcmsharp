using System.Runtime.InteropServices;

namespace DcmParse;

[StructLayout(LayoutKind.Auto)]
public readonly record struct DicomSequence(ushort Group, ushort Element, DicomSequenceItems Items);
