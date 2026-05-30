using DcmSharp.Parser;
using FluentAssertions;
using Xunit.Abstractions;

namespace DcmSharp.Tests;

[Collection(nameof(DicomParserCollection))]
public sealed class TestsForDicomParserWithStopping
{
    private readonly IDicomParser _dicomParser;

    public TestsForDicomParserWithStopping(DicomParserFixture fixture, ITestOutputHelper output)
    {
        _dicomParser = fixture.DicomParser;
        fixture.OutputHelper = output;
    }

    [Fact]
    public async Task ShouldParseExplicitValueRepresentationWithStop()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        var tag = DicomTags.SOPInstanceUID;
        var options = new DicomParserOptions
        {
            StopParsing = new StopParsingOptions
            {
                Group = tag.Group,
                Element = tag.Element,
                Depth = 0,
            },
        };

        // Act + Assert
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(file, options);

        // Assert
        dicomDataset.Should().NotBeNull();

        // The stopping tag should be present
        dicomDataset.TryGetString(tag, out string? sopInstanceUID).Should().BeTrue();
        sopInstanceUID.Should().Be("2.25.332838821141227624838581964210008219211");

        // This tag comes after the stopping tag, so it should not be present
        dicomDataset
            .TryGetString(DicomTags.PlacerOrderNumberImagingServiceRequest, out _)
            .Should()
            .BeFalse();
    }

    [Fact]
    public async Task ShouldParseImplicitValueRepresentationWithStop()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ImplicitVR.dcm");
        var tag = DicomTags.SOPInstanceUID;
        var options = new DicomParserOptions
        {
            StopParsing = new StopParsingOptions
            {
                Group = tag.Group,
                Element = tag.Element,
                Depth = 0,
            },
        };

        // Act + Assert
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(file, options);

        // Assert
        dicomDataset.Should().NotBeNull();

        // The stopping tag should be present
        dicomDataset.TryGetString(tag, out string? sopInstanceUID).Should().BeTrue();
        sopInstanceUID.Should().Be("1.2.840.113619.2.1.2411.1031152382.365.1.736169244");

        // This tag comes after the stopping tag, so it should not be present
        dicomDataset.TryGetString(DicomTags.RescaleType, out _).Should().BeFalse();
    }

    [Fact]
    public async Task ShouldParseNestedSequencesWithStop()
    {
        // Arrange
        var file = new FileInfo("./Dicom/TestPatternPalette.dcm");
        var tag = DicomTags.CodeMeaning;
        var options = new DicomParserOptions
        {
            StopParsing = new StopParsingOptions
            {
                Group = tag.Group,
                Element = tag.Element,
                Depth = 2,
            },
        };
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(file, options);

        // Act + Assert
        dicomDataset
            .TryGetSequence(
                DicomTags.SourceImageSequence,
                out ReadOnlyDicomDataset[]? sourceImageSequence
            )
            .Should()
            .BeTrue();
        sourceImageSequence.Should().NotBeNull();
        var firstSourceImage = sourceImageSequence![0];
        firstSourceImage.Should().NotBeNull();
        firstSourceImage
            .TryGetSequence(
                DicomTags.PurposeOfReferenceCodeSequence,
                out ReadOnlyDicomDataset[]? purposeOfReferenceCodeSequence
            )
            .Should()
            .BeTrue();
        purposeOfReferenceCodeSequence.Should().NotBeNull();
        var firstPurposeOfReferenceCodeSequence = purposeOfReferenceCodeSequence![0];
        firstPurposeOfReferenceCodeSequence
            .TryGetString(DicomTags.CodeMeaning, out string? codeMeaning)
            .Should()
            .BeTrue();
        codeMeaning.Should().Be("Uncompressed predecessor");
    }

    [Fact]
    public async Task ShouldParseExplicitSequenceAndSequenceItemsLengthsWithStop()
    {
        // Arrange
        var file = new FileInfo("./Dicom/Encoded.dcm");
        var tag = DicomTags.PlacerOrderNumberImagingServiceRequest;
        var options = new DicomParserOptions
        {
            StopParsing = new StopParsingOptions
            {
                Group = tag.Group,
                Element = tag.Element,
                Depth = 2,
            },
        };

        // Act
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(file, options);
        dicomDataset
            .TryGetString(DicomTags.PlacerOrderNumberImagingServiceRequest, out string? orderNumber)
            .Should()
            .BeTrue();

        // Assert
        orderNumber.Should().Be("ORDER2024112213363");
    }
}
