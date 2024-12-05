using System.Collections.Concurrent;

namespace DcmSharp.Memory;

internal sealed class DicomItemDictionaryPool
{
    private readonly int _maxPoolSize;

    private static readonly ConcurrentQueue<SortedDictionary<uint, ReadOnlyDicomItem>> _pool = new ConcurrentQueue<SortedDictionary<uint, ReadOnlyDicomItem>>();

    public DicomItemDictionaryPool(int maxPoolSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxPoolSize);
        _maxPoolSize = maxPoolSize;
    }

    internal SortedDictionary<uint, ReadOnlyDicomItem> Rent()
    {
        return _pool.TryDequeue(out var dictionary) ? dictionary : new SortedDictionary<uint, ReadOnlyDicomItem>(Comparer<uint>.Default);
    }

    internal void Return(SortedDictionary<uint, ReadOnlyDicomItem> dictionary)
    {
        if (_pool.Count >= _maxPoolSize)
        {
            return;
        }

        dictionary.Clear();
        _pool.Enqueue(dictionary);
    }
}
