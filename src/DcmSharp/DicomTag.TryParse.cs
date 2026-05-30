using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DcmSharp;

public sealed partial record DicomTag
{
    /// <summary>
    /// Parses a DICOM tag from a string in the following formats:
    /// <ul>
    ///     <li>(GGGG,EEEE)</li>
    ///     <li>GGGG,EEEE</li>
    ///     <li>GGGGEEEE</li>
    /// </ul>
    /// </summary>
    /// <param name="tagAsString">The tag as string.</param>
    /// <param name="tag">The parsed tag, or null if parsing failed.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool TryParse(
        ReadOnlySpan<char> tagAsString,
        [NotNullWhen(true)] out DicomTag? tag
    )
    {
        if (tagAsString.Length < 8)
        {
            tag = null;
            return false;
        }

        // Drop parentheses if present
        if (tagAsString[0] == '(' && tagAsString[^1] == ')')
        {
            tagAsString = tagAsString.Slice(1, tagAsString.Length - 2);
        }

        // Parse group
        ReadOnlySpan<char> groupSpan = tagAsString.Slice(0, 4);
        if (
            !ushort.TryParse(
                groupSpan,
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out ushort group
            )
        )
        {
            tag = null;
            return false;
        }

        // Parse element
        ReadOnlySpan<char> elementSpan =
            tagAsString[4] == ',' ? tagAsString.Slice(5, 4) : tagAsString.Slice(4, 4);
        if (
            !ushort.TryParse(
                elementSpan,
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out ushort element
            )
        )
        {
            tag = null;
            return false;
        }

        return DicomTagsIndex.TryLookup(group, element, out tag);
    }
}
