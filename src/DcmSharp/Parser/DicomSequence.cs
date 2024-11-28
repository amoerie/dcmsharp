using System.Runtime.InteropServices;
using DcmSharp.Memory;

namespace DcmSharp.Parser;

[StructLayout(LayoutKind.Auto)]
internal readonly record struct DicomSequence(ushort Group, ushort Element, DicomDatasets Items, long? EndPosition);

[StructLayout(LayoutKind.Auto)]
internal readonly record struct DicomSequenceItem(DicomDataset DicomDataset, long? EndPosition);
