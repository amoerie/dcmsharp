using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DcmSharp;

public sealed partial record DicomDataset
{
    public bool TryGetString(DicomTag tag, [NotNullWhen(true)] out string? value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        switch (item)
        {
            case DicomApplicationEntity { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomAgeString { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomAttributeTag { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomCodeString { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomDate { Value: { Length: > 0 } v }:
                value = v[0].ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                return true;
            case DicomDateTime { Value: { Length: > 0 } v }:
                value = v[0].ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                return true;
            case DicomDecimalString { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomFloatingPointDouble { Value: { Length: > 0 } v }:
                value = v[0].ToString(CultureInfo.InvariantCulture);
                return true;
            case DicomFloatingPointSingle { Value: { Length: > 0 } v }:
                value = v[0].ToString(CultureInfo.InvariantCulture);
                return true;
            case DicomIntegerString { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomLongString { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomLongText { Value: { } v }:
                value = v;
                return true;
            case DicomPersonName { Value: { Length: > 0 } v }:
                value = v[0].ToString();
                return true;
            case DicomShortString { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomShortText { Value: { } v }:
                value = v;
                return true;
            case DicomSignedLong { Value: { Length: > 0 } v }:
                value = v[0].ToString(CultureInfo.InvariantCulture);
                return true;
            case DicomSignedShort { Value: { Length: > 0 } v }:
                value = v[0].ToString(CultureInfo.InvariantCulture);
                return true;
            case DicomSignedVeryLong { Value: { Length: > 0 } v }:
                value = v[0].ToString(CultureInfo.InvariantCulture);
                return true;
            case DicomTime { Value: { Length: > 0 } v }:
                value = v[0].ToString("HHmmss", CultureInfo.InvariantCulture);
                return true;
            case DicomUnlimitedCharacters { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomUniqueIdentifier { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomUnlimitedText { Value: { } v }:
                value = v;
                return true;
            case DicomUniversalResource { Value: { } v }:
                value = v;
                return true;
            case DicomUrl { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
            case DicomUnsignedLong { Value: { Length: > 0 } v }:
                value = v[0].ToString(CultureInfo.InvariantCulture);
                return true;
            case DicomUnsignedShort { Value: { Length: > 0 } v }:
                value = v[0].ToString(CultureInfo.InvariantCulture);
                return true;
        }

        value = default;
        return false;
    }
}
