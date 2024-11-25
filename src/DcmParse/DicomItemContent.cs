using System.Runtime.InteropServices;
using DcmParse.Memory;

namespace DcmParse;

[StructLayout(LayoutKind.Auto)]
public readonly record struct DicomItemContent(
    ReadOnlyMemory<byte>? Value,
    ReadOnlyDicomFragments? Fragments,
    ReadOnlyDicomDatasets? SequenceItems)
{
    public static DicomItemContent Create(ReadOnlyMemory<byte> value) => new DicomItemContent(Value: value, Fragments: null, SequenceItems: null);
    public static DicomItemContent Create(ReadOnlyDicomFragments fragments) => new DicomItemContent(Value: null, Fragments: fragments, SequenceItems: null);
    public static DicomItemContent Create(ReadOnlyDicomDatasets sequenceItems) => new DicomItemContent(Value: null, Fragments: null, SequenceItems: sequenceItems);
}


