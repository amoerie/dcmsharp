namespace DcmSharp.Parser;

/// <summary>
/// Configuration options for the DICOM parser
/// </summary>
public sealed record DicomParserOptions
{
    /// <summary>
    /// The default parser options
    /// </summary>
    public static readonly DicomParserOptions Default = new DicomParserOptions();

    /// <summary>
    /// Stops the parsing process after a specific tag is completely parsed.
    /// If the tag is not present, parsing will stop after the first subsequent tag.
    /// </summary>
    public StopParsingOptions? StopParsing { get; init; }
}

/// <summary>
/// Stops the parsing process after a specific tag is completely parsed.
/// If the tag is not present, parsing will stop after the first subsequent tag.
/// </summary>
public sealed record StopParsingOptions
{
    /// <summary>
    /// The group number of the tag to stop parsing at
    /// </summary>
    public required ushort Group { get; init; }

    /// <summary>
    /// The element number of the tag to stop parsing at.
    /// </summary>
    public required ushort Element { get; init; }

    /// <summary>
    /// The depth of the group/tag to stop parsing at. Parsing will stop after the group/tag at this depth is completely parsed.
    /// A depth of 0 means the top-level dataset, 1 means within the first level of sequences, and so on.
    /// </summary>
    public ushort Depth { get; init; } = 0;
}
