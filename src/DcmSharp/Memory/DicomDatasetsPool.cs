using System.Buffers;

namespace DcmSharp.Memory;

internal sealed class DicomDatasetsPool
{
    private readonly ArrayPool<DicomDataset> _pool;

    public DicomDatasetsPool(int maxArrayLength, int maxArraysPerBucket)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxArrayLength);
        ArgumentOutOfRangeException.ThrowIfNegative(maxArraysPerBucket);
        _pool = ArrayPool<DicomDataset>.Create(maxArrayLength, maxArraysPerBucket);
    }

    internal DicomDataset[] Rent(int minimumLength)
    {
        return _pool.Rent(minimumLength);
    }

    internal void Return(DicomDataset[] datasets)
    {
        _pool.Return(datasets);
    }
}
