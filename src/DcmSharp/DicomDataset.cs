namespace DcmSharp;

public sealed record DicomDataset
{
    private readonly SortedDictionary<uint, DicomItem> _items = new SortedDictionary<uint, DicomItem>();

    public DicomDataset() {}

    public DicomItem this[DicomTag tag]
    {
        get => _items[(uint)tag.Group << 16 | tag.Element];
        set => _items[(uint)tag.Group << 16 | tag.Element] = value;
    }

    public DicomItem this[ushort group, ushort element]
    {
        get => _items[(uint)group << 16 | element];
        set => _items[(uint)group << 16 | element] = value;
    }
}
