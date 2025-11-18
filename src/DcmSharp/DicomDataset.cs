namespace DcmSharp;

public sealed partial record DicomDataset
{
    private readonly SortedDictionary<uint, IDicomItem> _items = new SortedDictionary<uint, IDicomItem>();

    public DicomDataset() {}


}
