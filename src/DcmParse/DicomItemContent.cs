namespace DcmParser;

public readonly record struct DicomItemContent(
    Memory<byte>? Data,
    IList<Memory<byte>>? Fragments,
    IList<DicomDataset>? Items)
{
    public static DicomItemContent Create(Memory<byte> data) => new DicomItemContent(data, null, null);
    public static DicomItemContent Create(IList<Memory<byte>> fragments) => new DicomItemContent(null, fragments, null);
    public static DicomItemContent Create(IList<DicomDataset> items) => new DicomItemContent(null, null, items);
}


