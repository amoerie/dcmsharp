namespace DcmSharp.Memory;

public readonly struct ReadOnlyDicomFragments : IDisposable
{
    private readonly DicomFragmentsPool _pool;
    private readonly ReadOnlyMemory<byte>[] _fragments;
    private readonly int _length;

    internal ReadOnlyDicomFragments(DicomFragmentsPool pool, ReadOnlyMemory<byte>[] fragments, int length)
    {
        _pool = pool;
        _fragments = fragments ?? throw new ArgumentNullException(nameof(fragments));
        _length = length;
    }

    public ReadOnlyMemory<ReadOnlyMemory<byte>> Fragments => _fragments.AsMemory(0, _length);

    public void Dispose()
    {
        Array.Clear(_fragments);
        _pool.Return(_fragments);
    }
}
