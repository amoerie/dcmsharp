using System.Collections;
using System.Text;
using DcmSharp.Memory;
using DcmSharp.Parser;

namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset
    : IReadOnlyDictionary<uint, ReadOnlyDicomItem>,
        IDisposable
{
    private readonly DicomItemDictionaryPool _pool;
    private readonly DicomMemories _memories;
    private readonly DicomValueParser _valueParser;
    private readonly SortedDictionary<uint, ReadOnlyDicomItem> _items;
    private readonly ReadOnlyDicomDatasetMetaData _metaData;

    internal ReadOnlyDicomDataset(
        DicomItemDictionaryPool dicomItemDictionaryPool,
        DicomMemories memories,
        DicomValueParser valueParser
    )
        : this()
    {
        _pool =
            dicomItemDictionaryPool
            ?? throw new ArgumentNullException(nameof(dicomItemDictionaryPool));
        _memories = memories ?? throw new ArgumentNullException(nameof(memories));
        _valueParser = valueParser ?? throw new ArgumentNullException(nameof(valueParser));
        _items = _pool.Rent();
        _metaData = new ReadOnlyDicomDatasetMetaData();
    }

    #region Encoding

    public Encoding Encoding
    {
        get => _metaData.Encoding;
        internal set => _metaData.Encoding = value;
    }

    #endregion

    #region Internal methods used by the parser

    internal void AddMemory(DicomMemory memory) => _memories.Add(memory);

    internal void Add(ushort group, ushort element, ReadOnlyDicomItem item) =>
        _items.Add((uint)group << 16 | element, item);

    #endregion

    #region IReadOnlyDictionary Implementation

    public IEnumerator<KeyValuePair<uint, ReadOnlyDicomItem>> GetEnumerator() =>
        _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _items.Count;

    public bool ContainsKey(uint key) => _items.ContainsKey(key);

    public bool TryGetValue(uint key, out ReadOnlyDicomItem value) =>
        _items.TryGetValue(key, out value);

    public ReadOnlyDicomItem this[uint key] => _items[key];
    public IEnumerable<uint> Keys => _items.Keys;
    public IEnumerable<ReadOnlyDicomItem> Values => _items.Values;

    #endregion

    #region Disposal

    public void Dispose()
    {
        foreach (var item in _items.Values)
        {
            if (item.Content.SequenceItems is { } sequenceItems)
            {
                sequenceItems.Dispose();
            }
        }

        _pool.Return(_items);
        _memories.Dispose();
    }

    #endregion
}
