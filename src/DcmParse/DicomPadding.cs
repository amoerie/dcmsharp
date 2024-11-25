using System.Runtime.CompilerServices;

namespace DcmParse;

internal static class DicomPadding
{
    private const byte Zero = 0x00;
    private const byte Space = 0x20;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<byte> TrimEndSpaces(ReadOnlySpan<byte> source)
    {
        return source.TrimEnd(Space);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<byte> TrimEndZero(ReadOnlySpan<byte> source)
    {
        return source.TrimEnd(Zero);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<char> TrimEndSpaces(ReadOnlySpan<char> source)
    {
        return source.TrimEnd(' ');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<byte> TrimSpaces(ReadOnlySpan<byte> source)
    {
        return source.Trim(Space);
    }
}
