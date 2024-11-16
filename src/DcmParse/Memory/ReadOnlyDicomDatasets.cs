namespace DcmParse.Memory;

public sealed class ReadOnlyDicomDatasets : IDisposable
{
    private readonly DicomDatasetsPool _datasetsPool;
    private readonly DicomDataset[] _datasets;
    private readonly int _length;
    private int _disposed;

    internal ReadOnlyDicomDatasets(DicomDatasetsPool datasetsPool, DicomDataset[] datasets, int length)
    {
        _datasetsPool = datasetsPool;
        _datasets = datasets ?? throw new ArgumentNullException(nameof(datasets));
        _length = length;
    }

    public ReadOnlyMemory<DicomDataset> Datasets
    {
        get
        {
            ThrowIfDisposed();
            return _datasets.AsMemory(0, _length);
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(Interlocked.CompareExchange(ref _disposed, 0, 0) == 1, this);

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
        {
            return;
        }

        for (int i = 0; i < _length; i++)
        {
            _datasets[i].Dispose();
        }

        Array.Clear(_datasets);
        _datasetsPool.Return(_datasets);
    }
}
