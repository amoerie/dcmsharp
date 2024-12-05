namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetPersonNames(DicomTag tag, out DicomPersonName[] values)
        => TryGetPersonNames(tag.Group, tag.Element, out values);

    public bool TryGetPersonNames(ushort group, ushort element, out DicomPersonName[] values)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            values = [];
            return false;
        }

        var encoding = Encoding;

        switch (vr)
        {
            case DicomVR.PN:
                return _valueParser.PN.TryParseAll(memory.Value.Span, encoding, out values);
        }

        values = [];
        return false;
    }
}
