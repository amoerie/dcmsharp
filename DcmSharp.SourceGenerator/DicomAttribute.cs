namespace DcmSharp.SourceGenerator;

public sealed class DicomAttribute
{
    public string? Tag { get; set; }
    public string? Name { get; set; }
    public string? Keyword { get; set; }
    public string? ValueRepresentation { get; set; }
    public string? ValueMultiplicity { get; set; }
    public string? Retired { get; set; }
    public string? Id { get; set; }
}
