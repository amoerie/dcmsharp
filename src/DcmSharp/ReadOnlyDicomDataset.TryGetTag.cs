using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGetTag(DicomTag tag, [NotNullWhen(true)] out DicomTag? value)
        => TryGetTag(tag.Group, tag.Element, out value);

    public bool TryGetTag(ushort group, ushort element, [NotNullWhen(true)] out DicomTag? value)
    {
        if (!TryGetMemory(group, element, out ReadOnlyMemory<byte>? memory, out DicomVR? vr))
        {
            value = default;
            return false;
        }

        switch (vr)
        {
            case DicomVR.AT:
                return _valueParser.AT.TryParse(memory.Value.Span, out value);
        }

        value = default;
        return false;
    }
}
