namespace DcmOrganize;

public class PatternException : Exception
{
    public PatternException() { }
    public PatternException(string? message) : base(message) { }
    public PatternException(string? message, Exception? innerException) : base(message, innerException) { }
}
