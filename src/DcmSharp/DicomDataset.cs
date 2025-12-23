namespace DcmSharp;

public sealed partial record DicomDataset
{
    private readonly SortedDictionary<uint, IDicomItem> _items = new SortedDictionary<uint, IDicomItem>();

    public DicomDataset() {}

    public DicomDataset(ReadOnlyDicomDataset readOnlyDicomDataset)
        => throw new NotSupportedException("Constructor not implemented yet");
}
