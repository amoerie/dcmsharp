namespace DcmSharp.Memory;

internal sealed class DicomFragments
{
    private readonly DicomFragmentsPool _fragmentsPool;
    private ReadOnlyMemory<byte>[] _fragments;
    private int _index;
    private int _readonly;

    public DicomFragments(DicomFragmentsPool fragmentsPool)
    {
        _fragmentsPool = fragmentsPool;
        _fragments = _fragmentsPool.Rent(256);
    }

    internal ReadOnlyDicomFragments ToReadOnly()
    {
        if (Interlocked.CompareExchange(ref _readonly, 1, 0) == 1)
        {
            throw new InvalidOperationException("Fragment has already been made read-only");
        }

        return new ReadOnlyDicomFragments(_fragmentsPool, _fragments, _index);
    }

    internal void Add(ReadOnlyMemory<byte> dataset)
    {
        if (Interlocked.CompareExchange(ref _readonly, 0, 0) == 1)
        {
            throw new InvalidOperationException(
                "Fragment has been made read-only and can no longer be modified"
            );
        }

        lock (_fragments)
        {
            if (_index >= _fragments.Length)
            {
                var fragments = _fragmentsPool.Rent(_fragments.Length * 2);
                Array.Copy(_fragments, fragments, _fragments.Length);
                Array.Clear(_fragments);
                _fragmentsPool.Return(_fragments);
                _fragments = fragments;
            }

            _fragments[_index] = dataset;
            _index++;
        }
    }
}
