using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmParse;

public static class DicomDatasetExtensionsTryGetString
{
    public static bool TryGetString(this DicomDataset dataset, DicomTag tag, [NotNullWhen(true)] out string? value)
        => TryGetString(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetString(this DicomDataset dataset, ushort group, ushort element, [NotNullWhen(true)] out string? value)
    {
        // TODO support encoding
        if (!dataset.TryGetRaw(group, element, out ReadOnlyMemory<byte>? raw))
        {
            value = null;
            return false;
        }

        value = Encoding.ASCII.GetString(raw.Value.Span);
        return true;
    }
}
