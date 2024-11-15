using System.Collections.Concurrent;

namespace DcmParse;

public sealed class DicomSequenceItemsPool
{
    private readonly int _maxPoolSize;
    private readonly int _initialCapacity;

    private static readonly ConcurrentQueue<List<DicomDataset>> _pool = new ConcurrentQueue<List<DicomDataset>>();

    public DicomSequenceItemsPool(int maxPoolSize, int initialCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxPoolSize);
        ArgumentOutOfRangeException.ThrowIfNegative(initialCapacity);
        _maxPoolSize = maxPoolSize;
        _initialCapacity = initialCapacity;
    }

    internal List<DicomDataset> Rent()
    {
        return _pool.TryDequeue(out var items) ? items : new List<DicomDataset>(_initialCapacity);
    }

    internal void Return(List<DicomDataset> items)
    {
        if (_pool.Count < _maxPoolSize)
        {
            items.Clear();
            _pool.Enqueue(items);
        }
    }
}
