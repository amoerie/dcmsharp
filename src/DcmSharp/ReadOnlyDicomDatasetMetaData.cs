using System.Text;

namespace DcmSharp;

public sealed record ReadOnlyDicomDatasetMetaData
{
    public Encoding Encoding { get; internal set; } = Encoding.ASCII;
}
