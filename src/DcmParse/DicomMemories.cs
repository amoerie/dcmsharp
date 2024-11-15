namespace DcmParse;

public sealed class DicomMemories : IDisposable
{
    private readonly DicomMemoriesPool _memoriesPool;
    private DicomMemory[] _memories;
    private int _index;

    public DicomMemories(DicomMemoriesPool memoriesPool)
    {
        _memoriesPool = memoriesPool;
        _memories = _memoriesPool.Rent(256);
    }

    public void Add(DicomMemory memory)
    {
        if (_index >= _memories.Length)
        {
            var memories = _memoriesPool.Rent(_memories.Length * 2);
            Array.Copy(_memories, memories, _memories.Length);
            Array.Clear(_memories);
            _memoriesPool.Return(_memories);
            _memories = memories;
        }

        _memories[_index] = memory;
        _index++;
    }

    public void Dispose()
    {
        lock (_memories)
        {
            for (int i = 0; i < _index; i++)
            {
                _memories[i].Dispose();
            }

            Array.Clear(_memories);
            _index = 0;
            _memoriesPool.Return(_memories);
        }
    }
}
