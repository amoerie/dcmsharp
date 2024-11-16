using System.Diagnostics.CodeAnalysis;
using DcmParse.Memory;

namespace DcmParse;

public readonly record struct DicomDataset : IDisposable
{
    private readonly DicomItemDictionaryPool _pool;
    private readonly DicomMemories _memories;
    private readonly Dictionary<uint, DicomItem> _items;

    internal DicomDataset(DicomItemDictionaryPool dicomItemDictionaryPool, DicomMemories memories) : this()
    {
        _pool = dicomItemDictionaryPool ?? throw new ArgumentNullException(nameof(dicomItemDictionaryPool));
        _memories = memories;
        _items = _pool.Rent();
    }

    internal void ReleaseOnDispose(DicomMemory memory) => _memories.Add(memory);

    public void Add(DicomTag tag, DicomItem item) => _items.Add((uint)tag.Group << 16 | tag.Element, item);
    public void Add(ushort group, ushort element, DicomItem item) => _items.Add((uint)group << 16 | element, item);

    public bool TryGetRaw(DicomTag tag, [NotNullWhen(true)] out ReadOnlyMemory<byte>? value) =>
        TryGetRaw(tag.Group, tag.Element, out value);

    public bool TryGetSequence(DicomTag tag, [NotNullWhen(true)] out ReadOnlyMemory<DicomDataset>? value) =>
        TryGetSequence(tag.Group, tag.Element, out value);

    public bool TryGetRaw(ushort group, ushort element, [NotNullWhen(true)] out ReadOnlyMemory<byte>? value)
    {
        if(!_items.TryGetValue((uint)group << 16 | element, out var item))
        {
            value = null;
            return false;
        }

        if (item.Content.Data is { } rawData)
        {
            value = rawData;
            return true;
        }

        value = null;
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

        value = null;
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
