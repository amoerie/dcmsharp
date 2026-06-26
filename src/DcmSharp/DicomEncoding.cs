using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DcmSharp;

public static class DicomEncoding
{
    private static readonly IDictionary<string, string> _knownEncodingNames = new Dictionary<
        string,
        string
    >(StringComparer.OrdinalIgnoreCase)
    {
        { "ISO_IR 13", "shift_jis" }, // JIS X 0201 (Shift JIS)
        { "ISO_IR 100", "iso-8859-1" }, // Latin Alphabet No. 1
        { "ISO_IR 101", "iso-8859-2" }, // Latin Alphabet No. 2
        { "ISO_IR 109", "iso-8859-3" }, // Latin Alphabet No. 3
        { "ISO_IR 110", "iso-8859-4" }, // Latin Alphabet No. 4
        { "ISO_IR 126", "iso-8859-7" }, // Greek
        { "ISO_IR 127", "iso-8859-6" }, // Arabic
        { "ISO_IR 138", "iso-8859-8" }, // Hebrew
        { "ISO_IR 144", "iso-8859-5" }, // Cyrillic
        { "ISO_IR 148", "iso-8859-9" }, // Latin Alphabet No. 5 (Turkish)
        { "ISO_IR 149", "x-cp20949" }, // KS X 1001 (Hangul and Hanja)
        { "ISO_IR 166", "windows-874" }, // TIS 620-2533 (Thai)
        { "ISO_IR 192", "utf-8" }, // Unicode in UTF-8
        { "GBK", "GBK" }, // Chinese (Simplified)
        { "GB18030", "gb18030" }, // Chinese (supersedes GBK)
        { "ISO 2022 IR 6", "us-ascii" }, // ASCII
        { "ISO 2022 IR 13", "shift_jis" }, // JIS X 0201 (Shift JIS) Extended
        { "ISO 2022 IR 87", "iso-2022-jp" }, // JIS X 0208 (Kanji) Extended
        { "ISO 2022 IR 100", "iso-8859-1" }, // Latin Alphabet No. 1 Extended
        { "ISO 2022 IR 101", "iso-8859-2" }, // Latin Alphabet No. 2 Extended
        { "ISO 2022 IR 109", "iso-8859-3" }, // Latin Alphabet No. 3 Extended
        { "ISO 2022 IR 110", "iso-8859-4" }, // Latin Alphabet No. 4 Extended
        { "ISO 2022 IR 127", "iso-8859-6" }, // Arabic Extended
        { "ISO 2022 IR 126", "iso-8859-7" }, // Greek Extended
        { "ISO 2022 IR 138", "iso-8859-8" }, // Hebrew Extended
        { "ISO 2022 IR 144", "iso-8859-5" }, // Cyrillic Extended
        { "ISO 2022 IR 148", "iso-8859-9" }, // Latin Alphabet No. 5 (Turkish) Extended
        { "ISO 2022 IR 149", "x-cp20949" }, // KS X 1001 (Hangul and Hanja) Extended
        { "ISO 2022 IR 159", "iso-2022-jp" }, // JIS X 0212 (Kanji) Extended
        { "ISO 2022 IR 166", "windows-874" }, // TIS 620-2533 (Thai) Extended
        { "ISO 2022 IR 58", "gb2312" }, // Chinese (Simplified) Extended
        { "ISO 2022 GBK", "GBK" }, // Chinese (Simplified) Extended (supersedes GB2312)
    };

    /// <summary>
    /// The known encodings with character replacement fallback handlers.
    /// </summary>
    private static readonly IDictionary<string, Encoding> _knownEncodings = _knownEncodingNames
        .Select(kvp =>
        {
            try
            {
                Encoding encoding = Encoding.GetEncoding(kvp.Value);
                return new { SpecificCharacterSet = kvp.Key, Encoding = encoding };
            }
            catch (ArgumentException)
            {
                return null;
            }
        })
        .Where(e => e != null)
        .ToDictionary(
            entry => entry!.SpecificCharacterSet,
            entry => entry!.Encoding,
            StringComparer.OrdinalIgnoreCase
        );

    public static bool TryParse(
        string specificCharacterSet,
        [NotNullWhen(true)] out Encoding? encoding
    )
    {
        if (string.IsNullOrEmpty(specificCharacterSet))
        {
            encoding = default;
            return false;
        }

        if (_knownEncodings.TryGetValue(specificCharacterSet, out encoding))
        {
            return true;
        }

        // Also allow some common misspellings (ISO-IR ### or ISO IR ### instead of ISO_IR ###)
        string specificCharacterSetWithCommonMisspellingsFixed = specificCharacterSet
            .Replace("ISO IR", "ISO_IR")
            .Replace("ISO-IR", "ISO_IR");

        if (
            _knownEncodings.TryGetValue(
                specificCharacterSetWithCommonMisspellingsFixed,
                out encoding
            )
        )
        {
            return true;
        }

        encoding = default;
        return false;
    }
}
