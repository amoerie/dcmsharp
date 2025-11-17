namespace DcmSharp;

public sealed partial record DicomDataset
{
    public void Add(DicomItem item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        uint key = ((uint)item.Tag.Group << 16) | item.Tag.Element;
        _items.Add(key, item);
    }

    public void Add(DicomTag tag, string value) => Add(DicomItemFactory.CreateFromString(tag, value));
}
