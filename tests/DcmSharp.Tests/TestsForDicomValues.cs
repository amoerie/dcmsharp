using DcmSharp.Parser;
using FluentAssertions;
using Xunit.Abstractions;

namespace DcmSharp.Tests;

[Collection(nameof(DicomParserCollection))]
public sealed class TestsForDicomValues
{
    private readonly IDicomParser _dicomParser;

    public TestsForDicomValues(DicomParserFixture fixture, ITestOutputHelper output)
    {
        fixture.OutputHelper = output;
        _dicomParser = fixture.DicomParser;
    }

    [Fact]
    public async Task ShouldParseAE()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SourceApplicationEntityTitle, out string? aeTitle).Should().BeTrue();

        // Assert
        aeTitle.Should().Be("DcmAnonymize");
    }

    [Fact]
    public async Task ShouldParseAS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.PatientAge, out string? patientAge).Should().BeTrue();

        // Assert
        patientAge.Should().Be("030Y");
    }

    [Fact]
    public async Task ShouldParseCS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.Modality, out string? modality).Should().BeTrue();

        // Assert
        modality.Should().Be("CT");
    }

    [Fact]
    public async Task ShouldParseCSMulti()
    {
        // Arrange
        var file = new FileInfo("./Dicom/Encoded.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.ImageType, out string? imageTypes).Should().BeTrue();
        dicomDataset.TryGetStrings(DicomTags.ImageType, out string[]? imageTypesArray).Should().BeTrue();

        // Assert
        imageTypes.Should().Be("ORIGINAL\\PRIMARY\\AXIAL");
        imageTypesArray.Should().NotBeNull();
        imageTypesArray.Should().HaveCount(3);
        imageTypesArray![0].Should().Be("ORIGINAL");
        imageTypesArray[1].Should().Be("PRIMARY");
        imageTypesArray[2].Should().Be("AXIAL");
    }

    [Fact]
    public async Task ShouldParseDA()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetDate(DicomTags.StudyDate, out DateOnly studyDate).Should().BeTrue();

        // Assert
        studyDate.Should().Be(new DateOnly(2024, 8, 12));
    }

    [Fact]
    public async Task ShouldParseDS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetDouble(DicomTags.SliceThickness, out double sliceThickness).Should().BeTrue();

        // Assert
        sliceThickness.Should().Be(1.25);
    }

    [Fact]
    public async Task ShouldParseDSMulti()
    {
        // Arrange
        var file = new FileInfo("./Dicom/Encoded.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.ImagePositionPatient, out string? positions).Should().BeTrue();
        dicomDataset.TryGetDoubles(DicomTags.ImagePositionPatient, out double[]? positionsArray).Should().BeTrue();

        // Assert
        positions.Should().Be("-125.000\\-126.800\\0.000");
        positionsArray.Should().NotBeNull();
        positionsArray.Should().HaveCount(3);
        positionsArray![0].Should().Be(-125.0);
        positionsArray[1].Should().Be(-126.8);
        positionsArray[2].Should().Be(0.0);
    }


    [Fact]
    public async Task ShouldParseDT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetTime(DicomTags.StudyTime, out TimeOnly studyTime).Should().BeTrue();

        // Assert
        studyTime.Should().Be(new TimeOnly(16, 32, 14));
    }

    [Fact]
    public async Task ShouldParseFD()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetDouble(DicomTags.RevolutionTime, out double revolutionTime).Should().BeTrue();

        // Assert
        revolutionTime.Should().Be(0.8);
    }

    [Fact]
    public async Task ShouldParseFL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetFloat(DicomTags.TableSpeed, out float tableSpeed).Should().BeTrue();

        // Assert
        tableSpeed.Should().Be(13.28125f);
    }

    [Fact]
    public async Task ShouldParseIS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetInt(DicomTags.InstanceNumber, out int instanceNumber).Should().BeTrue();

        // Assert
        instanceNumber.Should().Be(166);
    }

    [Fact]
    public async Task ShouldParseLS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.Manufacturer, out string? manufacturer).Should().BeTrue();

        // Assert
        manufacturer.Should().Be("GE MEDICAL SYSTEMS");
    }

    [Fact]
    public async Task ShouldParsePN()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.ReferringPhysicianName, out string? physicianName).Should().BeTrue();

        // Assert
        physicianName.Should().Be("McNalley^Sharia");
    }

    [Fact]
    public async Task ShouldParseSH()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.AccessionNumber, out string? accessionNumber).Should().BeTrue();

        // Assert
        accessionNumber.Should().Be("CT2024081216322");
    }

    [Fact]
    public async Task ShouldParseSQ()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetSequence(DicomTags.ReferencedImageSequence, out var sequence).Should().BeTrue();
        var referencedItem = sequence![0];
        referencedItem.TryGetString(DicomTags.ReferencedSOPClassUID, out string? sopClassUID).Should().BeTrue();

        // Assert
        sopClassUID.Should().Be("1.2.840.10008.5.1.4.1.1.2");
    }

    [Fact]
    public async Task ShouldParseSS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetShort(DicomTags.PixelPaddingValue, out short pixelPaddingValue).Should().BeTrue();

        // Assert
        pixelPaddingValue.Should().Be(-2000);
    }

    [Fact]
    public async Task ShouldParseUI()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SOPInstanceUID, out string? sopInstanceUID).Should().BeTrue();

        // Assert
        sopInstanceUID.Should().Be("2.25.332838821141227624838581964210008219211");
    }

    [Fact]
    public async Task ShouldParseUS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetUShort(DicomTags.Rows, out ushort rows).Should().BeTrue();

        // Assert
        rows.Should().Be(512);
    }
}
