using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DcmParse;

public static class DicomDatasetExtensionsTryGetDate
{
    private static readonly string[] _formats =
    [
        "yyyyMMdd",
        "yyyy.MM.dd",
        "yyyy/MM/dd",
        "yyyy",
        "yyyyMM",
        "yyyy.MM",
    ];

    private static readonly int _minLength = _formats.Min(format => format.Length);
    private static readonly int _maxLength = _formats.Max(format => format.Length);

    public static bool TryGetDate(this DicomDataset dataset, DicomTag tag, [NotNullWhen(true)] out DateOnly? value)
    {
        if (tag.VR != DicomVR.DA)
        {
            value = default;
            return false;
        }

        if (!dataset.TryGetRaw(tag, out ReadOnlyMemory<byte>? raw) || raw.Value.Length < _minLength)
        {
            value = default;
            return false;
        }

        var span = raw.Value.Span;
        int length = Math.Min(_maxLength, span.Length);
        Span<char> charSpan = stackalloc char[length];
        for (int i = 0; i < length; i++)
        {
            charSpan[i] = (char)span[i];
        }

        if (DateOnly.TryParseExact(charSpan, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsedDate))
        {
            value = parsedDate;
            return true;
        }

        value = default;
        return false;
    }
}
