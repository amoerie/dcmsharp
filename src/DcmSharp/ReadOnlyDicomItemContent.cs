using System.Runtime.InteropServices;
using DcmSharp.Memory;

namespace DcmSharp;

[StructLayout(LayoutKind.Auto)]
public readonly record struct ReadOnlyDicomItemContent(
    ReadOnlyMemory<byte>? Bytes,
    ReadOnlyDicomFragments? Fragments,
    ReadOnlyDicomDatasets? SequenceItems)
{
    public static ReadOnlyDicomItemContent Create(ReadOnlyMemory<byte> bytes) => new ReadOnlyDicomItemContent(Bytes: bytes, Fragments: null, SequenceItems: null);
    public static ReadOnlyDicomItemContent Create(ReadOnlyDicomFragments fragments) => new ReadOnlyDicomItemContent(Bytes: null, Fragments: fragments, SequenceItems: null);
    public static ReadOnlyDicomItemContent Create(ReadOnlyDicomDatasets sequenceItems) => new ReadOnlyDicomItemContent(Bytes: null, Fragments: null, SequenceItems: sequenceItems);
}


