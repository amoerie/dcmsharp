using System.Collections.Concurrent;

namespace DcmSharp.Memory;

internal sealed class DicomItemDictionaryPool
{
    private readonly int _maxPoolSize;

    private static readonly ConcurrentQueue<SortedDictionary<uint, DicomItem>> _pool = new ConcurrentQueue<SortedDictionary<uint, DicomItem>>();

    public DicomItemDictionaryPool(int maxPoolSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxPoolSize);
        _maxPoolSize = maxPoolSize;
    }

    internal SortedDictionary<uint, DicomItem> Rent()
    {
        return _pool.TryDequeue(out var dictionary) ? dictionary : new SortedDictionary<uint, DicomItem>(Comparer<uint>.Default);
    }

    internal void Return(SortedDictionary<uint, DicomItem> dictionary)
    {
        if (_pool.Count >= _maxPoolSize)
        {
            return;
        }

        dictionary.Clear();
        _pool.Enqueue(dictionary);
    }
}
