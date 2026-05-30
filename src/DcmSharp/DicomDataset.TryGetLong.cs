using System.Globalization;

namespace DcmSharp;

public sealed partial record DicomDataset
{
    public bool TryGetLong(DicomTag tag, out long value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        switch (item)
        {
            case DicomIntegerString { Value: { Length: > 0 } v } when long.TryParse(v[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed):
                value = parsed;
                return true;
            case DicomSignedLong { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomSignedShort { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomSignedVeryLong { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomUnsignedLong { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomUnsignedShort { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
        }

        value = default;
        return false;
    }
}
