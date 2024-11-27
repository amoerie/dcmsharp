using System.Diagnostics.CodeAnalysis;
using System.Text;
using DcmParse.Memory;

namespace DcmParse;

public readonly partial record struct DicomDataset : IDisposable
{
    private readonly DicomItemDictionaryPool _pool;
    private readonly DicomMemories _memories;
    private readonly DicomValueParser _valueParser;
    private readonly Dictionary<uint, DicomItem> _items;
    private readonly DicomDatasetMetaData _metaData;

    internal DicomDataset(
        DicomItemDictionaryPool dicomItemDictionaryPool,
        DicomMemories memories,
        DicomValueParser valueParser) : this()
    {
        _pool = dicomItemDictionaryPool ?? throw new ArgumentNullException(nameof(dicomItemDictionaryPool));
        _memories = memories;
        _valueParser = valueParser;
        _items = _pool.Rent();
        _metaData = new DicomDatasetMetaData();
    }

    public Encoding Encoding
    {
        get => _metaData.Encoding;
        set => _metaData.Encoding = value;
    }

    internal void ReleaseOnDispose(DicomMemory memory) => _memories.Add(memory);

    internal void Add(DicomTag tag, DicomItem item) => _items.Add((uint)tag.Group << 16 | tag.Element, item);
    internal void Add(ushort group, ushort element, DicomItem item) => _items.Add((uint)group << 16 | element, item);

    public bool TryGetValue(DicomTag tag, [NotNullWhen(true)] out ReadOnlyMemory<byte>? value) =>
        TryGetValue(tag.Group, tag.Element, out value, out _);

    public bool TryGetValue(DicomTag tag, [NotNullWhen(true)] out ReadOnlyMemory<byte>? value, [NotNullWhen(true)] out DicomVR? vr) =>
        TryGetValue(tag.Group, tag.Element, out value, out vr);

    public bool TryGetSequence(DicomTag tag, [NotNullWhen(true)] out ReadOnlyMemory<DicomDataset>? value) =>
        TryGetSequence(tag.Group, tag.Element, out value);

    public bool TryGetFragments(DicomTag tag, [NotNullWhen(true)] out ReadOnlyMemory<ReadOnlyMemory<byte>>? value) =>
        TryGetFragments(tag.Group, tag.Element, out value);

    public bool TryGetValue(ushort group, ushort element, [NotNullWhen(true)] out ReadOnlyMemory<byte>? value, [NotNullWhen(true)] out DicomVR? vr)
    {
        if(!_items.TryGetValue((uint)group << 16 | element, out var item))
        {
            value = default;
            vr = default;
            return false;
        }

        if (item.Content.Value is { } dicomValue)
        {
            value = dicomValue;
            vr = item.VR;
            return true;
        }

        value = default;
        vr = default;
        return false;
    }

    public bool TryGetSequence(ushort group, ushort element, [NotNullWhen(true)] out ReadOnlyMemory<DicomDataset>? value)
    {
        if(!_items.TryGetValue((uint)group << 16 | element, out var item))
        {
            value = null;
            return false;
        }

        if (item.Content.SequenceItems is { } sequenceItems)
        {
            value = sequenceItems.Datasets;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetFragments(ushort group, ushort element, [NotNullWhen(true)] out ReadOnlyMemory<ReadOnlyMemory<byte>>? value)
    {
        if(!_items.TryGetValue((uint)group << 16 | element, out var item))
        {
            value = default;
            return false;
        }

        if (item.Content.Fragments is { } fragments)
        {
            value = fragments.Fragments;
            return true;
        }

        value = default;
        return false;
    }

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
