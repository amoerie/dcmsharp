namespace DcmSharp;

public sealed partial record DicomDataset
{
    public void Set(DicomTag<string> tag, string value) => Add(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<DateOnly> tag, DateOnly value) => Add(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<int> tag, int value) => Add(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<ushort> tag, ushort value) => Add(DicomItemFactory.Create(tag, value));

    public void Set(DicomTag<short> tag, short value) => Add(DicomItemFactory.Create(tag, (int)value));

    public void Set(DicomTag<uint> tag, uint value) => Add(DicomItemFactory.Create(tag, (int)value));

    public void Set(DicomTag<long> tag, long value) => Add(DicomItemFactory.Create(tag, (int)value));

    public void Set(DicomTag<float> tag, float value)
    {
        ushort group = tag.Group;
        ushort element = tag.Element;
        Add(new DicomFloatingPointSingle(group, element, [value]));
    }

    public void Set(DicomTag<double> tag, double value)
    {
        ushort group = tag.Group;
        ushort element = tag.Element;
        Add(new DicomFloatingPointDouble(group, element, [value]));
    }

    public void Set(DicomTag<PersonName> tag, PersonName value)
    {
        ushort group = tag.Group;
        ushort element = tag.Element;
        Add(new DicomPersonName(group, element, [value]));
    }
}
