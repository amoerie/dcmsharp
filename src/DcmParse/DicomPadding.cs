using System.Runtime.CompilerServices;

namespace DcmParse;

internal static class DicomPadding
{
    private const byte Space = 0x20;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<byte> TrimEndSpaces(ReadOnlySpan<byte> source)
    {
        return source.TrimEnd(Space);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<char> TrimEndSpaces(ReadOnlySpan<char> source)
    {
        return source.TrimEnd(' ');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<byte> TrimStartSpaces(ReadOnlySpan<byte> source)
    {
        return source.TrimStart(Space);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<byte> TrimSpaces(ReadOnlySpan<byte> source)
    {
        return source.Trim(Space);
    }
}
