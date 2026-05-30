using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DcmSharp.Parser;

[StructLayout(LayoutKind.Auto)]
public ref struct DicomByteBuffer
{
    private ReadOnlySpan<byte> Span;
    private SequenceReader<byte> Reader;

    public DicomByteBuffer(ReadOnlySequence<byte> sequence)
    {
        Span = sequence.IsSingleSegment ? sequence.FirstSpan : default;
        Reader = Span.IsEmpty ? new SequenceReader<byte>(sequence) : default;
    }

    public bool IsEmpty => !Span.IsEmpty ? Span.Length == 0 : Reader.End;

    public long Remaining => !Span.IsEmpty ? Span.Length : Reader.Remaining;

    public bool TryReadShort(ref long position, out short output)
    {
        if (!Span.IsEmpty)
        {
            if (Span.Length < sizeof(short))
            {
                output = default;
                return false;
            }

            output = Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(Span));
            Span = Span[sizeof(short)..];
            position += sizeof(short);

            if (!BitConverter.IsLittleEndian)
            {
                output = BinaryPrimitives.ReverseEndianness(output);
            }

            return true;
        }

        if (!Reader.TryReadLittleEndian(out output))
        {
            return false;
        }

        position += sizeof(short);
        return true;
    }

    public bool TryReadInt(ref long position, out int output)
    {
        if (!Span.IsEmpty)
        {
            if (Span.Length < sizeof(int))
            {
                output = default;
                return false;
            }

            output = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(Span));
            Span = Span[sizeof(int)..];
            position += sizeof(int);

            if (!BitConverter.IsLittleEndian)
            {
                output = BinaryPrimitives.ReverseEndianness(output);
            }

            return true;
        }

        if (!Reader.TryReadLittleEndian(out output))
        {
            return false;
        }

        position += sizeof(int);
        return true;
    }

    public bool TryRead(ref long position, Span<byte> output)
    {
        if (!Span.IsEmpty)
        {
            if (output.Length > Span.Length)
            {
                return false;
            }

            Span[..output.Length].CopyTo(output);
            Span = Span[output.Length..];
            position += output.Length;
            return true;
        }

        if (!Reader.TryCopyTo(output))
        {
            return false;
        }

        position += output.Length;
        Reader.Advance(output.Length);
        return true;
    }

    public bool TryReadVr(ref long position, out byte b1, out byte b2)
    {
        if (!Span.IsEmpty)
        {
            if (Span.Length < 2)
            {
                b1 = default;
                b2 = default;
                return false;
            }

            b1 = Span[0];
            b2 = Span[1];
            Span = Span[2..];
            position += 2;

            if (!BitConverter.IsLittleEndian)
            {
                (b1, b2) = (b2, b1);
            }

            return true;
        }

        if (Reader.Remaining < 2)
        {
            b1 = default;
            b2 = default;
            return false;
        }

        Reader.TryRead(out b1);
        Reader.TryRead(out b2);

        if (!BitConverter.IsLittleEndian)
        {
            (b1, b2) = (b2, b1);
        }

        position += 2;

        return true;
    }

    public bool TryReadExplicitVrLongValueLength(ref long position, out int output)
    {
        if (!Span.IsEmpty)
        {
            if (Span.Length < 6)
            {
                output = default;
                return false;
            }

            output = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(Span.Slice(2, 6)));
            Span = Span[6..];
            position += 6;

            if (!BitConverter.IsLittleEndian)
            {
                output = BinaryPrimitives.ReverseEndianness(output);
            }

            return true;
        }

        if (Reader.Remaining < 6)
        {
            output = default;
            return false;
        }

        Reader.Advance(2);

        if (!Reader.TryReadLittleEndian(out output))
        {
            return false;
        }

        position += 6;
        return true;
    }

    public bool TryReadImplicitVrLongValueLength(ref long position, out int output)
    {
        if (!Span.IsEmpty)
        {
            if (Span.Length < 4)
            {
                output = default;
                return false;
            }

            output = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(Span));
            Span = Span[4..];
            position += 4;

            if (!BitConverter.IsLittleEndian)
            {
                output = BinaryPrimitives.ReverseEndianness(output);
            }

            return true;
        }

        if (Reader.Remaining < 4)
        {
            output = default;
            return false;
        }

        if (!Reader.TryReadLittleEndian(out output))
        {
            return false;
        }

        position += 4;
        return true;
    }
}
