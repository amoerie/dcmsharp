using System.Buffers;

namespace DcmParser;

public sealed class DicomMemoriesPool
{
    private readonly ArrayPool<DicomMemory> _pool;

    public DicomMemoriesPool(int maxArrayLength, int maxArraysPerBucket)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxArrayLength);
        ArgumentOutOfRangeException.ThrowIfNegative(maxArraysPerBucket);
        _pool = ArrayPool<DicomMemory>.Create(maxArrayLength, maxArraysPerBucket);
    }

    internal DicomMemory[] Rent(int minimumLength)
    {
        return _pool.Rent(minimumLength);
    }

    internal void Return(DicomMemory[] memories)
    {
        _pool.Return(memories);
    }
}
