namespace DcmSharp.Memory;

internal sealed class DicomDatasets
{
    private readonly DicomDatasetsPool _datasetsPool;
    private DicomDataset[] _datasets;
    private int _index;
    private int _readonly;

    public DicomDatasets(DicomDatasetsPool datasetsPool)
    {
        _datasetsPool = datasetsPool;
        _datasets = _datasetsPool.Rent(256);
    }

    internal ReadOnlyDicomDatasets ToReadOnly()
    {
        if (Interlocked.CompareExchange(ref _readonly, 1, 0) == 1)
        {
            throw new InvalidOperationException("Dataset has already been made read-only");
        }

        return new ReadOnlyDicomDatasets(_datasetsPool, _datasets, _index);
    }

    internal void Add(DicomDataset dataset)
    {
        if (Interlocked.CompareExchange(ref _readonly, 0, 0) == 1)
        {
            throw new InvalidOperationException("Dataset has been made read-only and can no longer be modified");
        }

        lock (_datasets)
        {
            if (_index >= _datasets.Length)
            {
                var datasets = _datasetsPool.Rent(_datasets.Length * 2);
                Array.Copy(_datasets, datasets, _datasets.Length);
                Array.Clear(_datasets);
                _datasetsPool.Return(_datasets);
                _datasets = datasets;
            }

            _datasets[_index] = dataset;
            _index++;
        }
    }
}
