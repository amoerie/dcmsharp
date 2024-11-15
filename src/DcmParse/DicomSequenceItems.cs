namespace DcmParse;

public readonly record struct DicomSequenceItems : IDisposable
{
    private readonly DicomSequenceItemsPool _pool;
    private readonly List<DicomDataset> _items;

    public DicomSequenceItems(DicomSequenceItemsPool pool)
    {
        _pool = pool ?? throw new ArgumentNullException(nameof(pool));
        _items = pool.Rent();
    }

    internal void Add(DicomDataset item) => _items.Add(item);

    public IReadOnlyList<DicomDataset> Items => _items;

    public void Dispose()
    {
        _pool.Return(_items);
    }
}
