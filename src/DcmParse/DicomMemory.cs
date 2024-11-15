using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DcmParse;

[DebuggerDisplay("{ToString(),raw}")]
public readonly struct DicomMemory : IMemoryOwner<byte>
{
    /// <summary>
    /// The usable length within <see cref="_array"/>
    /// </summary>
    private readonly int _length;

    /// <summary>
    /// The <see cref="ArrayPool{T}"/> instance used to rent <see cref="_array"/>.
    /// </summary>
    private readonly ArrayPool<byte> _pool;

    /// <summary>
    /// The underlying array.
    /// </summary>
    private readonly byte[] _array;

    /// <summary>
    /// Initializes a new instance of the <see cref="DicomMemory"/> class with the specified parameters.
    /// </summary>
    /// <param name="length">The length of the new memory buffer to use.</param>
    /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
    internal DicomMemory(ArrayPool<byte> pool, int length)
    {
        _length = length;
        _pool = pool;
        _array = pool.Rent(length);
    }

    /// <summary>
    /// Gets the number of items in the current instance
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    public Memory<byte> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _array.AsMemory(0, _length);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _pool.Return(_array);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        // Same representation used in Span<T>
        return $"DicomMemory<byte>[{_length}]";
    }
}
