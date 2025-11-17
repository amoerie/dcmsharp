namespace DcmSharp.Parser;

public enum DicomParseStage: byte
{
    ParseGroup,
    ParseElement,
    ParseVR,
    ParseLength,
    ParseValue,
}
