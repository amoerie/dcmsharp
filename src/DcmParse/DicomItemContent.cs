using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DcmParse.Memory;

namespace DcmParse;

[SuppressMessage(category: "Design", checkId: "MA0016:Prefer using collection abstraction instead of implementation")]
[StructLayout(LayoutKind.Auto)]
public readonly record struct DicomItemContent(
    ReadOnlyMemory<byte>? Data,
    ReadOnlyDicomFragments? Fragments,
    ReadOnlyDicomDatasets? SequenceItems)
{
    public static DicomItemContent Create(ReadOnlyMemory<byte> data) => new DicomItemContent(Data: data, Fragments: null, SequenceItems: null);
    public static DicomItemContent Create(ReadOnlyDicomFragments fragments) => new DicomItemContent(Data: null, Fragments: fragments, SequenceItems: null);
    public static DicomItemContent Create(ReadOnlyDicomDatasets sequenceItems) => new DicomItemContent(Data: null, Fragments: null, SequenceItems: sequenceItems);
}


