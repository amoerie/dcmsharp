using System.Diagnostics.CodeAnalysis;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class ATParser
{
    private const int Length = 4;

    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out DicomTag? value)
    {
        if (span.Length != Length)
        {
            value = default;
            return false;
        }

        ushort group = BitConverter.ToUInt16(span[..2]);
        ushort element = BitConverter.ToUInt16(span[2..]);
        return DicomTagsIndex.TryLookup(group, element, out value);
    }

    public bool TryParse(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? value)
    {
        if (span.Length != Length)
        {
            value = default;
            return false;
        }

        value = $"{span[1]:X2}{span[0]:X2}{span[3]:X2}{span[2]:X2}";
        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out DicomTag[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        int numberOfValues = span.Length / Length;
        values = new DicomTag[numberOfValues];
        for (int i = 0; i < numberOfValues; i++)
        {
            int offset = i * Length;
            ReadOnlySpan<byte> groupSpan = span.Slice(offset, 2);
            ReadOnlySpan<byte> elementSpan = span.Slice(offset + 2, 2);
            ushort group = BitConverter.ToUInt16(groupSpan);
            ushort element = BitConverter.ToUInt16(elementSpan);
            if (!DicomTagsIndex.TryLookup(group, element, out var value))
            {
                return false;
            }
            values[i] = value;
        }

        return true;
    }

    public bool TryParseAll(ReadOnlySpan<byte> span, out string[] values)
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new string[span.Length / Length];
        for (int i = 0; i < values.Length; i++)
        {
            int offset = i * Length;
            ReadOnlySpan<byte> valueSpan = span.Slice(offset, Length);
            values[i] = $"{valueSpan[1]:X2}{valueSpan[0]:X2}{valueSpan[3]:X2}{valueSpan[2]:X2}";
        }

        return true;
    }
}
