using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmParse.ValueRepresentations;

public static class DicomDatasetExtensionsTryGetLS
{
    public static bool TryGetLS(this DicomDataset dataset, DicomTag tag, [NotNullWhen(true)] out string? value)
        => TryGetLS(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetLS(this DicomDataset dataset, ushort group, ushort element, [NotNullWhen(true)] out string? value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? raw, out DicomVR? vr) || vr != DicomVR.LO)
        {
            value = default;
            return false;
        }

        // Trim trailing spaces and convert to string
        value = Encoding.ASCII.GetString(DicomPadding.TrimEndSpaces(raw.Value.Span));
        return true;
    }
}
