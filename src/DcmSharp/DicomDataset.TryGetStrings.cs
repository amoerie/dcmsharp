using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public readonly partial record struct DicomDataset
{
    public bool TryGetStrings(DicomTag tag, [NotNullWhen(true)] out string[]? values)
        => TryGetStrings(tag.Group, tag.Element, out values);

    public bool TryGetStrings(ushort group, ushort element, [NotNullWhen(true)] out string[]? values)
    {
        if (!TryGetString(group, element, out string? value))
        {
            values = default;
            return false;
        }

        values = string.IsNullOrEmpty(value)
            ? []
            : value.Split('\\');

        return true;
    }
}
