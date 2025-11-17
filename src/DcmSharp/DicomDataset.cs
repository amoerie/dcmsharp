namespace DcmSharp;

public sealed partial record DicomDataset
{
    private readonly SortedDictionary<uint, DicomItem> _items = new SortedDictionary<uint, DicomItem>();

    public DicomDataset() {}
}
