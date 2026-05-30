namespace DcmSharp;

public sealed partial record DicomDataset
{
    public void Clear() => _items.Clear();
}
