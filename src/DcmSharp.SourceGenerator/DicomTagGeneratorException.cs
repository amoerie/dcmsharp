namespace DcmSharp.SourceGenerator;

public class DicomTagGeneratorException : Exception
{
    public DicomTagGeneratorException(string message)
        : base(message) { }

    public DicomTagGeneratorException(string message, Exception innerException)
        : base(message, innerException) { }
}
