using DcmSharp;

namespace DcmFind
{
    public static class DicomTagParser
    {
        public static bool TryParse(string dicomTagAsString, out DicomTag? dicomTag)
        {
            if (string.IsNullOrEmpty(dicomTagAsString))
            {
                Console.Error.WriteLine($"Invalid empty DICOM tag ''");
                dicomTag = null;
                return false;
            }

            if (dicomTagAsString[0] == '(' || char.IsDigit(dicomTagAsString[0]))
            {
                if (DicomTag.TryParse(dicomTagAsString, out dicomTag))
                {
                    return true;
                }

                Console.Error.WriteLine($"Invalid DICOM tag '{dicomTagAsString}'");
                return false;
            }

            if (DicomTagsIndex.TryLookupByKeyword(dicomTagAsString, out dicomTag))
            {
                return true;
            }

            Console.Error.WriteLine($"Invalid DICOM tag '{dicomTagAsString}'");
            return false;
        }
    }
}
