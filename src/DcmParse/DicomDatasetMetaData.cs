using System.Text;

namespace DcmParse;

public sealed record DicomDatasetMetaData
{
    public Encoding Encoding { get; set; } = Encoding.ASCII;
}
