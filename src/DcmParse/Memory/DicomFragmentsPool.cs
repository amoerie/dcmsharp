using System.Buffers;

namespace DcmParse.Memory;

internal sealed class DicomFragmentsPool
{
    private readonly ArrayPool<ReadOnlyMemory<byte>> _pool;

    public DicomFragmentsPool(int maxArrayLength, int maxArraysPerBucket)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxArrayLength);
        ArgumentOutOfRangeException.ThrowIfNegative(maxArraysPerBucket);
        _pool = ArrayPool<ReadOnlyMemory<byte>>.Create(maxArrayLength, maxArraysPerBucket);
    }

    internal ReadOnlyMemory<byte>[] Rent(int minimumLength)
    {
        return _pool.Rent(minimumLength);
    }

    internal void Return(ReadOnlyMemory<byte>[] fragments)
    {
        _pool.Return(fragments);
    }
}
