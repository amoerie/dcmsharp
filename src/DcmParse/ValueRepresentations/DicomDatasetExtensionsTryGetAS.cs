using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmParse.ValueRepresentations;

public static class DicomDatasetExtensionsTryGetAS
{
    public static bool TryGetAS(this DicomDataset dataset, DicomTag tag, [NotNullWhen(true)] out string? value)
        => TryGetAS(dataset, tag.Group, tag.Element, out value);

    public static bool TryGetAS(this DicomDataset dataset, ushort group, ushort element, [NotNullWhen(true)] out string? value)
    {
        if (!dataset.TryGetValue(group, element, out ReadOnlyMemory<byte>? raw, out DicomVR? vr) || vr != DicomVR.AS)
        {
            value = default;
            return false;
        }

        ReadOnlySpan<byte> span = DicomPadding.TrimEndSpaces(raw.Value.Span);
        if (span.Length != 4)
        {
            value = default;
            return false;
        }

        value = Encoding.ASCII.GetString(span);
        return true;
    }
}
