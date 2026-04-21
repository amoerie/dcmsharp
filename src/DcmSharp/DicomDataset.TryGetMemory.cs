using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public sealed partial record DicomDataset
{
    public bool TryGetMemory(DicomTag tag, [NotNullWhen(true)] out ReadOnlyMemory<byte>? value, [NotNullWhen(true)] out DicomVR? vr)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            vr = default;
            return false;
        }

        value = ReadOnlyMemory<byte>.Empty;
        vr = tag.ValueRepresentation;
        return true;
    }
}
