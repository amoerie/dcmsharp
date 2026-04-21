using System.Diagnostics.CodeAnalysis;

namespace DcmSharp;

public static class DicomTagsIndex
{
    private static readonly Dictionary<uint, DicomTag> _indexByGroupAndElement = BuildByGroupAndElement();
    private static Dictionary<uint, DicomTag> BuildByGroupAndElement()
    {
        var dict = new Dictionary<uint, DicomTag>();
        foreach (var tag in DicomTags.All)
            dict.TryAdd(((uint)tag.Group << 16) | tag.Element, tag);
        return dict;
    }

    private static readonly Dictionary<string, DicomTag> _indexByKeyword = BuildByKeyword();
    private static Dictionary<string, DicomTag> BuildByKeyword()
    {
        var dict = new Dictionary<string, DicomTag>(StringComparer.OrdinalIgnoreCase);
        foreach (var tag in DicomTags.All)
            dict.TryAdd(tag.Keyword, tag);
        return dict;
    }

    /// <summary>
    /// Lookup a <see cref="DicomTag"/> by its group and element.
    /// </summary>
    /// <param name="group">The group</param>
    /// <param name="element">The element</param>
    /// <param name="dicomTag">The resulting DicomTag if found; otherwise, null.</param>
    /// <returns>True if found; otherwise, false.</returns>
    public static bool TryLookup(ushort group, ushort element, [NotNullWhen(true)] out DicomTag? dicomTag)
    {
        if (!_indexByGroupAndElement.TryGetValue(((uint)group << 16) | element, out var tag))
        {
            dicomTag = null;
            return false;
        }

        dicomTag = tag;
        return true;
    }

    /// <summary>
    /// Lookup a <see cref="DicomTag"/> by its group and element.
    /// </summary>
    /// <param name="keyword">The name of the DICOM tag, e.g. "AccessionNumber". See <see cref="DicomTags"/> for possible names</param>
    /// <param name="dicomTag">The resulting DicomTag if found; otherwise, null.</param>
    /// <returns>True if found; otherwise, false.</returns>
    public static bool TryLookupByKeyword(string keyword, [NotNullWhen(true)] out DicomTag? dicomTag)
    {
#pragma warning disable CS0612 // Type or member is obsolete
        if (!_indexByKeyword.TryGetValue(keyword, out var tag))
#pragma warning restore CS0612 // Type or member is obsolete
        {
            dicomTag = null;
            return false;
        }

        dicomTag = tag;
        return true;
    }
}
