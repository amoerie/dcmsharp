namespace DcmSharp;

public sealed partial record DicomDataset
{
    public bool TryGetDateTime(DicomTag tag, out DateTime value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        switch (item)
        {
            case DicomDate { Value: { Length: > 0 } v }:
                value = v[0].ToDateTime(TimeOnly.MinValue);
                return true;
            case DicomDateTime { Value: { Length: > 0 } v }:
                value = v[0];
                return true;
        }

        value = default;
        return false;
    }
}
