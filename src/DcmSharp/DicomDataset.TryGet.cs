namespace DcmSharp;

public sealed partial record DicomDataset
{
    public bool TryGet(DicomTag tag, out IDicomItem? item)
    {
        uint key = ((uint)tag.Group << 16) | tag.Element;
        return _items.TryGetValue(key, out item);
    }
}
