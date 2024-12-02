using System.Numerics;

namespace DcmSharp.Parser.ValueRepresentations;

internal sealed class FDParser
{
    private const int Length = 8;

    public bool TryParse<TNumber>(ReadOnlySpan<byte> span, out TNumber value) where TNumber: struct, INumber<TNumber>
    {
        if (span.Length != Length)
        {
            value = default;
            return false;
        }

        double number = BitConverter.ToDouble(span);
        try
        {
            value = TNumber.CreateChecked(number);
            return true;
        }
        catch (OverflowException)
        {
            value = default;
            return false;
        }

    }

    public bool TryParseAll<TNumber>(ReadOnlySpan<byte> span, out TNumber[] values) where TNumber: struct, INumber<TNumber>
    {
        if (span.Length % Length != 0)
        {
            values = [];
            return false;
        }

        values = new TNumber[Length];
        int numberOfValues = span.Length / Length;
        for (int i = 0; i < numberOfValues; i++)
        {
            int offset = i * Length;
            double value = BitConverter.ToDouble(span.Slice(offset, Length));
            try
            {
                TNumber number = TNumber.CreateChecked(value);
                values[i] = number;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        return true;
    }
}
