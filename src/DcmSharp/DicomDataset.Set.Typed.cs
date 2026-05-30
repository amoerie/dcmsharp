namespace DcmSharp;

public sealed partial record DicomDataset
{
    private void SetItem(IDicomItem item)
    {
        uint key = ((uint)item.Group << 16) | item.Element;
        _items[key] = item;
    }

    public void Set(DicomTag<string> tag, string value) => SetItem(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<DateOnly> tag, DateOnly value) => SetItem(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<int> tag, int value) => SetItem(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<ushort> tag, ushort value) => SetItem(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<short> tag, short value)
        => SetItem(new DicomSignedShort(tag.Group, tag.Element, [value]));

    public void Set(DicomTag<uint> tag, uint value)
        => SetItem(new DicomUnsignedLong(tag.Group, tag.Element, [value]));

    public void Set(DicomTag<long> tag, long value)
        => SetItem(new DicomSignedVeryLong(tag.Group, tag.Element, [value]));

    public void Set(DicomTag<float> tag, float value)
        => SetItem(new DicomFloatingPointSingle(tag.Group, tag.Element, [value]));

    public void Set(DicomTag<double> tag, double value)
        => SetItem(new DicomFloatingPointDouble(tag.Group, tag.Element, [value]));

    public void Set(DicomTag<PersonName> tag, PersonName value)
        => SetItem(new DicomPersonName(tag.Group, tag.Element, [value]));
}
