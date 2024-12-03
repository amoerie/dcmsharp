using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetTag(DicomTag tag, [NotNullWhen(true)] out DicomTag? value)
        => TryGetTag(tag.Group, tag.Element, out value);

    public bool TryGetTag(ushort group, ushort element, [NotNullWhen(true)] out DicomTag? value)
    {
        if (!TryGetValue(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.AT:
                if(_valueParser.AT.TryParse(memory.Value.Span, out ushort tagGroup, out ushort tagElement)
                   && DicomTagsIndex.TryLookup(tagGroup, tagElement, out DicomTag? tag))
                {
                    value = tag;
                    return true;
                }

                break;
        }

        value = default;
        return false;
    }
}
