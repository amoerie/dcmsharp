using System.IO.Compression;
using System.Text.Json;

namespace DcmSharp.SourceGenerator;

/// <summary>
/// Loads the known DICOM attributes from the embedded standard.zip resource.
/// To update, download the latest version from https://github.com/innolitics/dicom-standard
/// </summary>
internal static class DicomAttributesLoader
{
    private static JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
    };

    public static List<DicomAttribute> LoadAttributesFromZip()
    {
        var assembly = typeof(DicomTagsGenerator).Assembly;
        string? resourceName = assembly
            .GetManifestResourceNames()
            .SingleOrDefault(n => n.EndsWith("standard.zip"));

        if (resourceName is null)
        {
            throw new DicomTagGeneratorException("Embedded resource 'standard.zip' not found");
        }

        using var stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new DicomTagGeneratorException(
                "Embedded resource 'standard.zip' cannot be opened"
            );
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        var entry = archive.Entries.FirstOrDefault(e => e.Name == "attributes.json");
        if (entry == null)
        {
            throw new DicomTagGeneratorException(
                "Embedded resource 'standard.zip' does not contain file named 'attributes.json'"
            );
        }

        using var entryStream = entry.Open();
        using var reader = new StreamReader(entryStream);
        string json = reader.ReadToEnd();
        try
        {
            var dicomAttributes = JsonSerializer.Deserialize<List<DicomAttribute>>(
                json,
                JsonSerializerOptions
            );
            if (dicomAttributes is null)
            {
                throw new DicomTagGeneratorException("Deserialized 'attributes.json' is null");
            }

            return dicomAttributes;
        }
        catch (JsonException e)
        {
            throw new DicomTagGeneratorException("Failed to deserialize 'attributes.json'", e);
        }
    }
}
