namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetPersonName(DicomTag tag, out DicomPersonName value)
        => TryGetPersonName(tag.Group, tag.Element, out value);

    public bool TryGetPersonName(ushort group, ushort element, out DicomPersonName value)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        var encoding = Encoding;

        switch (vr)
        {
            case DicomVR.PN:
                if(_valueParser.PN.TryParse(memory.Value.Span, encoding, out DicomPersonName pnValue))
                {
                    value = pnValue;
                    return true;
                }

                break;
        }

        value = default;
        return false;
    }
}
