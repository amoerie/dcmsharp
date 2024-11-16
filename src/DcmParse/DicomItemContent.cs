using System.Diagnostics.CodeAnalysis;
using DcmParse.Memory;

namespace DcmParse;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation")]
public readonly record struct DicomItemContent(
    ReadOnlyMemory<byte>? Data,
    IReadOnlyList<Memory<byte>>? Fragments,
    ReadOnlyDicomDatasets? SequenceItems)
{
    public static DicomItemContent Create(ReadOnlyMemory<byte> data) => new DicomItemContent(data, null, null);
    public static DicomItemContent Create(IReadOnlyList<Memory<byte>> fragments) => new DicomItemContent(null, fragments, null);
    public static DicomItemContent Create(ReadOnlyDicomDatasets sequenceItems) => new DicomItemContent(null, null, sequenceItems);
}


