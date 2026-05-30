namespace DcmSharp.Memory;

public readonly struct ReadOnlyDicomDatasets : IDisposable
{
    private readonly DicomDatasetsPool _datasetsPool;
    private readonly ReadOnlyDicomDataset[] _datasets;
    private readonly int _length;

    internal ReadOnlyDicomDatasets(
        DicomDatasetsPool datasetsPool,
        ReadOnlyDicomDataset[] datasets,
        int length
    )
    {
        _datasetsPool = datasetsPool;
        _datasets = datasets ?? throw new ArgumentNullException(nameof(datasets));
        _length = length;
    }

    public ReadOnlyMemory<ReadOnlyDicomDataset> Datasets => _datasets.AsMemory(0, _length);

    public void Dispose()
    {
        for (int i = 0; i < _length; i++)
        {
            _datasets[i].Dispose();
        }

        Array.Clear(_datasets);
        _datasetsPool.Return(_datasets);
    }
}
