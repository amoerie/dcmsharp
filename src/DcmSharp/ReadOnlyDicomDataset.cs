using System.Text;
using DcmSharp.Memory;
using DcmSharp.Parser;

namespace DcmSharp;

public readonly partial record struct ReadOnlyDicomDataset : IDisposable
{
    private readonly DicomItemDictionaryPool _pool;
    private readonly DicomMemories _memories;
    private readonly DicomValueParser _valueParser;
    private readonly SortedDictionary<uint, ReadOnlyDicomItem> _items;
    private readonly DicomDatasetMetaData _metaData;

    internal ReadOnlyDicomDataset(
        DicomItemDictionaryPool dicomItemDictionaryPool,
        DicomMemories memories,
        DicomValueParser valueParser) : this()
    {
        _pool = dicomItemDictionaryPool ?? throw new ArgumentNullException(nameof(dicomItemDictionaryPool));
        _memories = memories ?? throw new ArgumentNullException(nameof(memories));
        _valueParser = valueParser ?? throw new ArgumentNullException(nameof(valueParser));
        _items = _pool.Rent();
        _metaData = new DicomDatasetMetaData();
    }

    public Encoding Encoding
    {
        get => _metaData.Encoding;
        internal set => _metaData.Encoding = value;
    }

    internal void AddMemory(DicomMemory memory) => _memories.Add(memory);

    internal void Add(ushort group, ushort element, ReadOnlyDicomItem item) => _items.Add((uint)group << 16 | element, item);

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
}
