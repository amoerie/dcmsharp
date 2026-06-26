using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
{
    public bool TryGet(DicomTag<string> tag, [NotNullWhen(true)] out string? value) =>
        TryGetString(tag, out value);

    public bool TryGet(DicomTag<string[]> tag, [NotNullWhen(true)] out string[]? value) =>
        TryGetStrings(tag, out value);

    public bool TryGet(DicomTag<DateOnly> tag, out DateOnly value) => TryGetDate(tag, out value);

    public bool TryGet(DicomTag<DateOnly[]> tag, [NotNullWhen(true)] out DateOnly[]? value) =>
        TryGetDates(tag, out value);

    public bool TryGet(DicomTag<TimeOnly> tag, out TimeOnly value) => TryGetTime(tag, out value);

    public bool TryGet(DicomTag<TimeOnly[]> tag, [NotNullWhen(true)] out TimeOnly[]? value) =>
        TryGetTimes(tag, out value);

    public bool TryGet(DicomTag<DateTime> tag, out DateTime value) =>
        TryGetDateTime(tag, out value);

    public bool TryGet(DicomTag<DateTime[]> tag, [NotNullWhen(true)] out DateTime[]? value) =>
        TryGetDateTimes(tag, out value);

    public bool TryGet(DicomTag<short> tag, out short value) => TryGetShort(tag, out value);

    public bool TryGet(DicomTag<short[]> tag, [NotNullWhen(true)] out short[]? value) =>
        TryGetShorts(tag, out value);

    public bool TryGet(DicomTag<ushort> tag, out ushort value) => TryGetUShort(tag, out value);

    public bool TryGet(DicomTag<ushort[]> tag, [NotNullWhen(true)] out ushort[]? value) =>
        TryGetUShorts(tag, out value);

    public bool TryGet(DicomTag<int> tag, out int value) => TryGetInt(tag, out value);

    public bool TryGet(DicomTag<int[]> tag, [NotNullWhen(true)] out int[]? value) =>
        TryGetInts(tag, out value);

    public bool TryGet(DicomTag<uint> tag, out uint value) => TryGetUInt(tag, out value);

    public bool TryGet(DicomTag<uint[]> tag, [NotNullWhen(true)] out uint[]? value) =>
        TryGetUInts(tag, out value);

    public bool TryGet(DicomTag<long> tag, out long value) => TryGetLong(tag, out value);

    public bool TryGet(DicomTag<long[]> tag, [NotNullWhen(true)] out long[]? value) =>
        TryGetLongs(tag, out value);

    public bool TryGet(DicomTag<ulong> tag, out ulong value) => TryGetULong(tag, out value);

    public bool TryGet(DicomTag<ulong[]> tag, [NotNullWhen(true)] out ulong[]? value) =>
        TryGetULongs(tag, out value);

    public bool TryGet(DicomTag<float> tag, out float value) => TryGetFloat(tag, out value);

    public bool TryGet(DicomTag<float[]> tag, [NotNullWhen(true)] out float[]? value) =>
        TryGetFloats(tag, out value);

    public bool TryGet(DicomTag<double> tag, out double value) => TryGetDouble(tag, out value);

    public bool TryGet(DicomTag<double[]> tag, [NotNullWhen(true)] out double[]? value) =>
        TryGetDoubles(tag, out value);

    public bool TryGet(DicomTag<PersonName> tag, out PersonName value) =>
        TryGetPersonName(tag, out value);

    public bool TryGet(DicomTag<PersonName[]> tag, [NotNullWhen(true)] out PersonName[]? value)
    {
        if (TryGetPersonNames(tag, out PersonName[] values))
        {
            value = values;
            return true;
        }
        value = default;
        return false;
    }

    public bool TryGet(
        DicomTag<ReadOnlyMemory<byte>> tag,
        [NotNullWhen(true)] out ReadOnlyMemory<byte>? value
    )
    {
        if (TryGetMemory(tag, out ReadOnlyMemory<byte>? mem, out _))
        {
            value = mem;
            return true;
        }
        value = default;
        return false;
    }
}
