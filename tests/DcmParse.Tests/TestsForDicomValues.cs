using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DcmParse.Tests;

public sealed class TestsForDicomValues : IDisposable
{
    private readonly ServiceProvider _services;
    private readonly IDicomParser _dicomParser;

    public TestsForDicomValues(ITestOutputHelper output)
    {
        _services = new ServiceCollection()
            .AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddXUnit(output);
            })
            .AddDcmParse()
            .BuildServiceProvider();
        _dicomParser = _services.GetRequiredService<IDicomParser>();
    }

    public void Dispose()
    {
        _services.Dispose();
    }

    [Fact]
    public async Task ShouldParseDicomUid()
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
    public async Task ShouldParseDicomCodeString()
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
    public async Task ShouldParseDicomDate()
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
    public async Task ShouldParseDicomTime()
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
    public async Task ShouldParseDicomDecimalString()
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
    public async Task ShouldParseDicomIntegerString()
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
    public async Task ShouldParseDicomPersonName()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.ReferringPhysicianName, out string? physicianName).Should().BeTrue();

        // Assert
        physicianName.Should().Be("Sharia McNalley");
    }

    [Fact]
    public async Task ShouldParseDicomUnsignedShort()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetUShort(DicomTags.Rows, out ushort rows).Should().BeTrue();

        // Assert
        rows.Should().Be(512);
    }

    [Fact]
    public async Task ShouldParseDicomLongString()
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
    public async Task ShouldParseDicomFloatingPointDouble()
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
    public async Task ShouldParseDicomApplicationEntity()
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
    public async Task ShouldParseDicomShortString()
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
    public async Task ShouldParseDicomSequence()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetSequence(DicomTags.ReferencedImageSequence, out var sequence).Should().BeTrue();
        var referencedItem = sequence!.Value.Span[0];
        referencedItem.TryGetString(DicomTags.ReferencedSOPClassUID, out string? sopClassUID).Should().BeTrue();

        // Assert
        sopClassUID.Should().Be("1.2.840.10008.5.1.4.1.1.2");
    }

    [Fact]
    public async Task ShouldParseDicomFloatingPointSingle()
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
    public async Task ShouldParseDicomSignedLong()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        // TODO
        // dicomDataset.TryGetInt32(DicomTags.UnknownTagAndData1, out int unknownData).Should().BeTrue();

        // Assert
        // unknownData.Should().Be(1617883207);
    }

    [Fact]
    public async Task ShouldParseDicomSignedShort()
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
    public async Task ShouldParseDicomAgeString()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.PatientAge, out string? patientAge).Should().BeTrue();

        // Assert
        patientAge.Should().Be("030Y");
    }
}
