using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public interface IDicomDataset
{
    bool TryGetString(DicomTag tag, [NotNullWhen(true)] out string? value);
    bool TryGetLong(DicomTag tag, out long value);
    bool TryGetDateTime(DicomTag tag, out DateTime value);
    bool TryGetMemory(DicomTag tag, [NotNullWhen(true)] out ReadOnlyMemory<byte>? value, [NotNullWhen(true)] out DicomVR? vr);
}
