using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DcmParse;

public static class DicomDatasetExtensionsTryGetTime
{
    private static readonly string[] _formats =
    [
        "HHmmss",
        "HH",
        "HHmm",
        "HHmmssf",
        "HHmmssff",
        "HHmmssfff",
        "HHmmssffff",
        "HHmmssfffff",
        "HHmmssffffff",
        "HHmmss.f",
        "HHmmss.ff",
        "HHmmss.fff",
        "HHmmss.ffff",
        "HHmmss.fffff",
        "HHmmss.ffffff",
        "HH.mm",
        "HH.mm.ss",
        "HH.mm.ss.f",
        "HH.mm.ss.ff",
        "HH.mm.ss.fff",
        "HH.mm.ss.ffff",
        "HH.mm.ss.fffff",
        "HH.mm.ss.ffffff",
        "HH:mm",
        "HH:mm:ss",
        "HH:mm:ss:f",
        "HH:mm:ss:ff",
        "HH:mm:ss:fff",
        "HH:mm:ss:ffff",
        "HH:mm:ss:fffff",
        "HH:mm:ss:ffffff",
        "HH:mm:ss.f",
        "HH:mm:ss.ff",
        "HH:mm:ss.fff",
        "HH:mm:ss.ffff",
        "HH:mm:ss.fffff",
        "HH:mm:ss.ffffff",
    ];

    private static readonly int _minLength = _formats.Min(format => format.Length);
    private static readonly int _maxLength = _formats.Max(format => format.Length);

    public static bool TryGetTime(this DicomDataset dataset, DicomTag tag, [NotNullWhen(true)] out TimeOnly? value)
    {
        if (tag.VR != DicomVR.TM)
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

        if (TimeOnly.TryParseExact(charSpan, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out TimeOnly parsedTime))
        {
            value = parsedTime;
            return true;
        }

        value = default;
        return false;
    }
}
