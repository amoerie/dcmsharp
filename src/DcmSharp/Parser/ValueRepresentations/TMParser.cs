using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class TMParser
{
    private const int MaxLength = 16;

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

    public bool TryParse(ReadOnlySpan<byte> span, out TimeOnly value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimEndSpaces(span);
        Span<char> charSpan = stackalloc char[Math.Min(MaxLength, trimmedSpan.Length)];
        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];

        if (TimeOnly.TryParseExact(charSpan, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out TimeOnly parsedTime))
        {
            value = parsedTime;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = Encoding.ASCII.GetString(DicomPadding.TrimEndSpaces(span));
        return true;
    }

    [SkipLocalsInit]
    public bool TryParseAll(ReadOnlySpan<byte> span, out TimeOnly[] values)
    {
        if (span.IsEmpty)
        {
            values = [];
            return false;
        }

        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimEndSpaces(span);

        char[]? sharedChars = null;
        Span<char> charSpan = trimmedSpan.Length < 255
            ? stackalloc char[trimmedSpan.Length]
            : sharedChars = ArrayPool<char>.Shared.Rent(trimmedSpan.Length);

        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];

        int numberOfValues = charSpan.Count('\\') + 1;
        Range[]? sharedRanges = null;
        Span<Range> ranges = numberOfValues < 16
            ? stackalloc Range[numberOfValues]
            : sharedRanges = ArrayPool<Range>.Shared.Rent(numberOfValues);
        MemoryExtensions.Split(charSpan, ranges, '\\');

        values = new TimeOnly[numberOfValues];

        bool allOk = true;
        for (int i = 0; i < ranges.Length; i++)
        {
            Range range = ranges[i];

            if (!TimeOnly.TryParseExact(charSpan[range], _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly parsedDate))
            {
                allOk = false;
                break;
            }

            values[i] = parsedDate;
        }

        if (sharedChars is not null)
        {
            ArrayPool<char>.Shared.Return(sharedChars);
        }
        if (sharedRanges is not null)
        {
            ArrayPool<Range>.Shared.Return(sharedRanges);
        }

        return allOk;
    }

    [SkipLocalsInit]
    public bool TryParseAll(ReadOnlySpan<byte> span, out string[] values)
    {
        if (span.IsEmpty)
        {
            values = [];
            return false;
        }

        ReadOnlySpan<byte> trimmedSpan = DicomPadding.TrimSpaces(span);

        char[]? sharedChars = null;
        Span<char> charSpan = trimmedSpan.Length < 255
            ? stackalloc char[trimmedSpan.Length]
            : sharedChars = ArrayPool<char>.Shared.Rent(trimmedSpan.Length);

        int written = Encoding.ASCII.GetChars(trimmedSpan, charSpan);
        charSpan = charSpan[..written];

        int numberOfValues = charSpan.Count('\\') + 1;
        Range[]? sharedRanges = null;
        Span<Range> ranges = numberOfValues < 16
            ? stackalloc Range[numberOfValues]
            : sharedRanges = ArrayPool<Range>.Shared.Rent(numberOfValues);
        MemoryExtensions.Split(charSpan, ranges, '\\');

        values = new string[numberOfValues];

        for (int i = 0; i < ranges.Length; i++)
        {
            Range range = ranges[i];
            values[i] = new string(charSpan[range]);
        }

        if (sharedChars is not null)
        {
            ArrayPool<char>.Shared.Return(sharedChars);
        }
        if (sharedRanges is not null)
        {
            ArrayPool<Range>.Shared.Return(sharedRanges);
        }

        return true;
    }

}
