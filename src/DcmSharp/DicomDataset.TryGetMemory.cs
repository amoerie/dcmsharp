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

        // Raw byte extraction is not implemented for in-memory item types yet.
        // Returning false is safer than reporting success with an empty buffer.
        value = default;
        vr = default;
        return false;
    }
}
