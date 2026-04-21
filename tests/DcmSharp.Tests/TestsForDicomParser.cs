using DcmSharp.Parser;
using FluentAssertions;

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
        dicomDataset.Should().NotBeNull();
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
        dicomDataset.Should().NotBeNull();
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
        dicomDataset.TryGetString(group, element, out string? actualValue).Should().BeTrue();

        // Assert
        actualValue.Should().Be(expectedValue);
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
        dicomDataset.TryGetString(group, element, out string? actualValue).Should().BeTrue();

        // Assert
        actualValue.Should().Be(expectedValue);
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
    public async Task ShouldParseExplicitSequenceAndSequenceItemsLengths()
    {
        // Arrange
        var file = new FileInfo("./Dicom/Encoded.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        dicomDataset
            .TryGetString(DicomTags.PlacerOrderNumberImagingServiceRequest, out string? orderNumber)
            .Should()
            .BeTrue();

        // Assert
        orderNumber.Should().Be("ORDER2024112213363");
    }
}
