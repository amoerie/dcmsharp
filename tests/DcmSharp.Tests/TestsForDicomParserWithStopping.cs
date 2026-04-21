using DcmSharp.Parser;

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
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            options,
            TestContext.Current.CancellationToken
        );

        // Assert

        // The stopping tag should be present
        Assert.True(dicomDataset.TryGetString(tag, out string? sopInstanceUID));
        Assert.Equal("2.25.332838821141227624838581964210008219211", sopInstanceUID);

        // This tag comes after the stopping tag, so it should not be present
        Assert.False(
            dicomDataset.TryGetString(DicomTags.PlacerOrderNumberImagingServiceRequest, out _)
        );
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
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            options,
            TestContext.Current.CancellationToken
        );

        // Assert

        // The stopping tag should be present
        Assert.True(dicomDataset.TryGetString(tag, out string? sopInstanceUID));
        Assert.Equal("1.2.840.113619.2.1.2411.1031152382.365.1.736169244", sopInstanceUID);

        // This tag comes after the stopping tag, so it should not be present
        Assert.False(dicomDataset.TryGetString(DicomTags.RescaleType, out _));
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
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            options,
            TestContext.Current.CancellationToken
        );

        // Act + Assert
        Assert.True(
            dicomDataset.TryGetSequence(
                DicomTags.SourceImageSequence,
                out ReadOnlyDicomDataset[]? sourceImageSequence
            )
        );
        Assert.NotNull(sourceImageSequence);
        var firstSourceImage = sourceImageSequence![0];
        Assert.True(
            firstSourceImage.TryGetSequence(
                DicomTags.PurposeOfReferenceCodeSequence,
                out ReadOnlyDicomDataset[]? purposeOfReferenceCodeSequence
            )
        );
        Assert.NotNull(purposeOfReferenceCodeSequence);
        var firstPurposeOfReferenceCodeSequence = purposeOfReferenceCodeSequence![0];
        Assert.True(
            firstPurposeOfReferenceCodeSequence.TryGetString(
                DicomTags.CodeMeaning,
                out string? codeMeaning
            )
        );
        Assert.Equal("Uncompressed predecessor", codeMeaning);
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
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            options,
            TestContext.Current.CancellationToken
        );
        Assert.True(
            dicomDataset.TryGetString(
                DicomTags.PlacerOrderNumberImagingServiceRequest,
                out string? orderNumber
            )
        );

        // Assert
        Assert.Equal("ORDER2024112213363", orderNumber);
    }
}
