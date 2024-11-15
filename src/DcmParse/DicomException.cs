namespace DcmParse;

public sealed class DicomException : Exception
{
    public DicomException(string? message) : base(message) { }
    public DicomException(string? message, Exception? innerException) : base(message, innerException) { }
}
