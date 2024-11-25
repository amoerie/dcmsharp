using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmParse.ValueRepresentations;

public static class DicomDatasetExtensionsTryGetCS
{
    public static bool TryGetCS(this DicomDataset dataset, DicomTag tag, [NotNullWhen(true)] out string? value)
        => TryGetCS(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetCS(this DicomDataset dataset, ushort group, ushort element, [NotNullWhen(true)] out string? value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? raw, out DicomVR? vr) || vr != DicomVR.CS)
        {
            value = default;
            return false;
        }

        ReadOnlySpan<byte> span = DicomPadding.TrimEndSpaces(raw.Value.Span);
        value = Encoding.ASCII.GetString(span);
        return true;
    }
}
