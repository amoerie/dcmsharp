namespace DcmSharp;

public sealed partial record DicomDataset
{
    private void AddOrUpdate(IDicomItem item)
    {
        uint key = ((uint)item.Group << 16) | item.Element;
        _items[key] = item;
    }

    public void Set(DicomTag<string> tag, string value) => AddOrUpdate(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<DateOnly> tag, DateOnly value) => AddOrUpdate(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<int> tag, int value) => AddOrUpdate(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<ushort> tag, ushort value) => AddOrUpdate(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<short> tag, short value) => AddOrUpdate(DicomItemFactory.Create(tag, (int)value));

    public void Set(DicomTag<uint> tag, uint value) => AddOrUpdate(DicomItemFactory.Create(tag, (int)value));

    public void Set(DicomTag<long> tag, long value) => AddOrUpdate(DicomItemFactory.Create(tag, (int)value));

    public void Set(DicomTag<float> tag, float value)
        => AddOrUpdate(new DicomFloatingPointSingle(tag.Group, tag.Element, [value]));

    public void Set(DicomTag<double> tag, double value)
        => AddOrUpdate(new DicomFloatingPointDouble(tag.Group, tag.Element, [value]));

    public void Set(DicomTag<PersonName> tag, PersonName value)
        => AddOrUpdate(new DicomPersonName(tag.Group, tag.Element, [value]));
}
