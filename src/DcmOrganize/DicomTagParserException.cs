namespace DcmOrganize;

public class DicomTagParserException : Exception
{
    public DicomTagParserException() { }
    public DicomTagParserException(string? message) : base(message) { }
    public DicomTagParserException(string? message, Exception? innerException) : base(message, innerException) { }
}
