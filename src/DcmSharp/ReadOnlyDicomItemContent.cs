using System.Runtime.InteropServices;
using DcmSharp.Memory;

namespace DcmSharp;

[StructLayout(LayoutKind.Auto)]
public readonly record struct ReadOnlyDicomItemContent(
    ReadOnlyMemory<byte>? Memory,
    ReadOnlyDicomFragments? Fragments,
    ReadOnlyDicomDatasets? SequenceItems
)
{
    public static ReadOnlyDicomItemContent Create(ReadOnlyMemory<byte> memory) =>
        new ReadOnlyDicomItemContent(Memory: memory, Fragments: null, SequenceItems: null);

    public static ReadOnlyDicomItemContent Create(ReadOnlyDicomFragments fragments) =>
        new ReadOnlyDicomItemContent(Memory: null, Fragments: fragments, SequenceItems: null);

    public static ReadOnlyDicomItemContent Create(ReadOnlyDicomDatasets sequenceItems) =>
        new ReadOnlyDicomItemContent(Memory: null, Fragments: null, SequenceItems: sequenceItems);
}
