using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetSequence(DicomTag tag, [NotNullWhen(true)] out DicomDataset[]? value) =>
        TryGetSequence(tag.Group, tag.Element, out value);

    public bool TryGetSequence(ushort group, ushort element, [NotNullWhen(true)] out DicomDataset[]? value)
    {
        if(!_items.TryGetValue((uint)group << 16 | element, out var item))
        {
            value = default;
            return false;
        }

        if (item.Content.SequenceItems is { } sequenceItems)
        {
            value = sequenceItems.Datasets.Span.ToArray();
            return true;
        }

        value = default;
        return false;
    }

}
