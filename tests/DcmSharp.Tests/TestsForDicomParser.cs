using DcmSharp.Parser;

namespace DcmSharp.Tests;

[Collection(nameof(DicomParserCollection))]
public sealed class TestsForDicomParser
{
    private readonly IDicomParser _dicomParser;

    public TestsForDicomParser(DicomParserFixture fixture, ITestOutputHelper output)
    {
        _dicomParser = fixture.DicomParser;
        fixture.OutputHelper = output;
    }

    [Fact]
    public async Task ShouldParseExplicitValueRepresentation()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");

        // Act + Assert
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Assert
    }

    [Fact]
    public async Task ShouldParseImplicitValueRepresentation()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ImplicitVR.dcm");

        // Act + Assert
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Assert
    }

    [Theory]
    [InlineData(0x0008, 0x0018, "2.25.332838821141227624838581964210008219211")]
    [InlineData(0x0040, 0x2016, "ORDER2024081216321")]
    public async Task ShouldRetrieveDicomTagFromExplicitVRDataset(
        ushort group,
        ushort element,
        string expectedValue
    )
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetString(group, element, out string? actualValue));

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData(0x0008, 0x0018, "1.2.840.113619.2.1.2411.1031152382.365.1.736169244")]
    [InlineData(0x0028, 0x1054, "US")]
    public async Task ShouldRetrieveDicomTagFromImplicitVRDataset(
        ushort group,
        ushort element,
        string expectedValue
    )
    {
        // Arrange
        var file = new FileInfo("./Dicom/ImplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetString(group, element, out string? actualValue));

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public async Task ShouldParseNestedSequences()
    {
        // Arrange
        var file = new FileInfo("./Dicom/TestPatternPalette.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
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
    public async Task ShouldParseExplicitSequenceAndSequenceItemsLengths()
    {
        // Arrange
        var file = new FileInfo("./Dicom/Encoded.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
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
