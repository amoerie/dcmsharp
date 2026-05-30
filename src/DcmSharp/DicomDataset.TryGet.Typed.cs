using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public sealed partial record DicomDataset
{
    public bool TryGet(DicomTag<string> tag, [NotNullWhen(true)] out string? value)
        => TryGetString(tag, out value);

    public bool TryGet(DicomTag<string[]> tag, [NotNullWhen(true)] out string[]? value)
    {
        // Delegate to TryGetString and wrap in array for now
        if (TryGetString(tag, out string? single))
        {
            value = [single];
            return true;
        }
        value = default;
        return false;
    }

    public bool TryGet(DicomTag<DateOnly> tag, out DateOnly value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        if (item is DicomDate { Value: { Length: > 0 } v })
        {
            value = v[0];
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGet(DicomTag<DateTime> tag, out DateTime value)
        => TryGetDateTime(tag, out value);

    public bool TryGet(DicomTag<short> tag, out short value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        if (item is DicomSignedShort { Value: { Length: > 0 } v })
        {
            value = v[0];
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGet(DicomTag<ushort> tag, out ushort value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        if (item is DicomUnsignedShort { Value: { Length: > 0 } v })
        {
            value = v[0];
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGet(DicomTag<int> tag, out int value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        if (item is DicomSignedLong { Value: { Length: > 0 } v })
        {
            value = v[0];
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGet(DicomTag<uint> tag, out uint value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        if (item is DicomUnsignedLong { Value: { Length: > 0 } v })
        {
            value = v[0];
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGet(DicomTag<long> tag, out long value)
        => TryGetLong(tag, out value);

    public bool TryGet(DicomTag<ulong> tag, out ulong value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        switch (item)
        {
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

    public bool TryGet(DicomTag<float> tag, out float value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        if (item is DicomFloatingPointSingle { Value: { Length: > 0 } v })
        {
            value = v[0];
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGet(DicomTag<double> tag, out double value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        if (item is DicomFloatingPointDouble { Value: { Length: > 0 } v })
        {
            value = v[0];
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGet(DicomTag<PersonName> tag, [NotNullWhen(true)] out PersonName? value)
    {
        if (!TryGet(tag, out IDicomItem? item))
        {
            value = default;
            return false;
        }

        if (item is DicomPersonName { Value: { Length: > 0 } v })
        {
            value = v[0];
            return true;
        }

        value = default;
        return false;
    }
}
