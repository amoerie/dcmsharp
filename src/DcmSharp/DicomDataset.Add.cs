namespace DcmSharp;

public sealed partial record DicomDataset
{
    public void Add(IDicomItem item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        uint key = ((uint)item.Group << 16) | item.Element;
        _items.Add(key, item);
    }

    public void Add(DicomTag tag, string value) => Add(DicomItemFactory.Create(tag, value));
    public void Add(DicomTag tag, int value) => Add(DicomItemFactory.Create(tag, value));
    public void Add(DicomTag tag, ushort value) => Add(DicomItemFactory.Create(tag, value));
    public void Add(DicomTag tag, DateOnly value) => Add(DicomItemFactory.Create(tag, value));
}
