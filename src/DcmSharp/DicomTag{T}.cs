namespace DcmSharp;

/// <summary>
/// A strongly-typed DICOM tag that encodes the CLR return type at compile time.
/// This enables type-safe access to tag values without requiring the caller to know the VR.
/// </summary>
/// <typeparam name="T">The CLR type that this tag's value resolves to.</typeparam>
public sealed record DicomTag<T>(
    ushort Group,
    ushort Element,
    DicomVR ValueRepresentation,
    DicomVR[] AdditionalValueRepresentations,
    DicomVM ValueMultiplicity,
    string Keyword,
    string Name
)
    : DicomTag(
        Group,
        Element,
        ValueRepresentation,
        AdditionalValueRepresentations,
        ValueMultiplicity,
        Keyword,
        Name
    );
