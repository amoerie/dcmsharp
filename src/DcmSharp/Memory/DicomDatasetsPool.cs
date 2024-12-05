using System.Buffers;

namespace DcmSharp.Memory;

internal sealed class DicomDatasetsPool
{
    private readonly ArrayPool<ReadOnlyDicomDataset> _pool;

    public DicomDatasetsPool(int maxArrayLength, int maxArraysPerBucket)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxArrayLength);
        ArgumentOutOfRangeException.ThrowIfNegative(maxArraysPerBucket);
        _pool = ArrayPool<ReadOnlyDicomDataset>.Create(maxArrayLength, maxArraysPerBucket);
    }

    internal ReadOnlyDicomDataset[] Rent(int minimumLength)
    {
        return _pool.Rent(minimumLength);
    }

    internal void Return(ReadOnlyDicomDataset[] datasets)
    {
        _pool.Return(datasets);
    }
}
