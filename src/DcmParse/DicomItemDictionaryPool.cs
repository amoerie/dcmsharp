using System.Collections.Concurrent;

namespace DcmParse;

public sealed class DicomItemDictionaryPool
{
    private readonly int _maxPoolSize;
    private readonly int _initialDicomDictionaryCapacity;

    private static readonly ConcurrentQueue<Dictionary<uint, DicomItem>> _pool = new ConcurrentQueue<Dictionary<uint, DicomItem>>();

    public DicomItemDictionaryPool(int maxPoolSize, int initialDicomDictionaryCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxPoolSize);
        ArgumentOutOfRangeException.ThrowIfNegative(initialDicomDictionaryCapacity);
        _maxPoolSize = maxPoolSize;
        _initialDicomDictionaryCapacity = initialDicomDictionaryCapacity;
    }

    internal Dictionary<uint, DicomItem> Rent()
    {
        return _pool.TryDequeue(out var dictionary) ? dictionary : new Dictionary<uint, DicomItem>(_initialDicomDictionaryCapacity);
    }

    internal void Return(Dictionary<uint, DicomItem> dictionary)
    {
        if (_pool.Count < _maxPoolSize)
        {
            dictionary.Clear();
            _pool.Enqueue(dictionary);
        }
    }
}
