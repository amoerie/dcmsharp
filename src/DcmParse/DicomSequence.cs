namespace DcmParser;

internal readonly record struct DicomSequence(ushort Group, ushort Element, List<DicomDataset> Items);