using System.Text;

namespace DcmSharp;

public sealed record DicomDatasetMetaData
{
    public Encoding Encoding { get; set; } = Encoding.ASCII;
}
