using DcmSharp.Parser;
using FluentAssertions;
using Xunit.Abstractions;

namespace DcmSharp.Tests;

[Collection(nameof(DicomParserCollection))]
public sealed class TestsForDicomSingleValues
{
    private readonly IDicomParser _dicomParser;

    public TestsForDicomSingleValues(DicomParserFixture fixture, ITestOutputHelper output)
    {
        fixture.OutputHelper = output;
        _dicomParser = fixture.DicomParser;
    }

    [Fact]
    public async Task ShouldParseAE()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SelectorAEValue, out string? value).Should().BeTrue();

        // Assert
        value.Should().Be("MODALITY1");
    }

    [Fact]
    public async Task ShouldParseAS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SelectorASValue, out string? value).Should().BeTrue();

        // Assert
        value.Should().Be("025Y");
    }

    [Fact]
    public async Task ShouldParseAT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetTag(DicomTags.SelectorATValue, out DicomTag? value).Should().BeTrue();

        // Assert
        value.Should().Be(DicomTags.TransferSyntaxUID);
    }

    [Fact]
    public async Task ShouldParseDA()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetDate(DicomTags.SelectorDAValue, out DateOnly value).Should().BeTrue();

        // Assert
        value.Should().Be(DateOnly.Parse("2024-12-03"));
    }

    [Fact]
    public async Task ShouldParseDS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetDouble(DicomTags.SelectorDSValue, out double value).Should().BeTrue();

        // Assert
        value.Should().Be(0.25);
    }

    [Fact]
    public async Task ShouldParseDT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetDateTime(DicomTags.SelectorDTValue, out DateTime value).Should().BeTrue();

        // Assert
        value.Should().Be(DateTime.Parse("2024-12-03T12:00:00"));
    }

    [Fact]
    public async Task ShouldParseFD()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetDouble(DicomTags.SelectorFDValue, out double value).Should().BeTrue();

        // Assert
        value.Should().Be(-100.123);
    }

    [Fact]
    public async Task ShouldParseFL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetFloat(DicomTags.SelectorFLValue, out float value).Should().BeTrue();

        // Assert
        value.Should().Be(100.5f);
    }

    [Fact]
    public async Task ShouldParseIS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetInt(DicomTags.SelectorISValue, out int value).Should().BeTrue();

        // Assert
        value.Should().Be(1);
    }

    [Fact]
    public async Task ShouldParseLO()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SelectorLOValue, out string? value).Should().BeTrue();

        // Assert
        value.Should().Be("Medical Center A");
    }

    [Fact]
    public async Task ShouldParseLT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SelectorLTValue, out string? value).Should().BeTrue();

        // Assert
        value.Should().Be("Some long notes");
    }

    [Fact]
    public async Task ShouldParsePN()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetPersonName(DicomTags.SelectorPNValue, out DicomPersonName value).Should().BeTrue();

        // Assert
        var expected = new DicomPersonName("Dr", "Smith", null, null, null, null, null, null, null);
        value.Should().Be(expected);
    }

    [Fact]
    public async Task ShouldParseSH()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SelectorSHValue, out string? value).Should().BeTrue();

        // Assert
        value.Should().Be("CT123");
    }

    [Fact]
    public async Task ShouldParseSL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetInt(DicomTags.SelectorSLValue, out int value).Should().BeTrue();

        // Assert
        value.Should().Be(-1);
    }

    [Fact]
    public async Task ShouldParseSS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetShort(DicomTags.SelectorSSValue, out short value).Should().BeTrue();

        // Assert
        value.Should().Be(-32768);
    }

    [Fact]
    public async Task ShouldParseST()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SelectorSTValue, out string? value).Should().BeTrue();

        // Assert
        value.Should().Be("History1");
    }

    [Fact]
    public async Task ShouldParseSV()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetLong(DicomTags.SelectorSVValue, out long value).Should().BeTrue();

        // Assert
        value.Should().Be(9223372036854775807);
    }

    [Fact]
    public async Task ShouldParseTM()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetTime(DicomTags.SelectorTMValue, out TimeOnly value).Should().BeTrue();

        // Assert
        value.Should().Be(TimeOnly.Parse("12:00:00"));
    }

    [Fact]
    public async Task ShouldParseUC()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SelectorUCValue, out string? value).Should().BeTrue();

        // Assert
        value.Should().Be("Device A");
    }

    [Fact]
    public async Task ShouldParseUI()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SelectorUIValue, out string? value).Should().BeTrue();

        // Assert
        value.Should().Be("1.2.3");
    }

    [Fact]
    public async Task ShouldParseUL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetUInt(DicomTags.SelectorULValue, out uint value).Should().BeTrue();

        // Assert
        value.Should().Be(4294967295);
    }

    [Fact]
    public async Task ShouldParseUS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetUShort(DicomTags.SelectorUSValue, out ushort value).Should().BeTrue();

        // Assert
        value.Should().Be(1);
    }

    [Fact]
    public async Task ShouldParseUT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SelectorUTValue, out string? value).Should().BeTrue();

        // Assert
        value.Should().Be("Modified");
    }

    [Fact]
    public async Task ShouldParseUV()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetULong(DicomTags.SelectorUVValue, out ulong value).Should().BeTrue();

        // Assert
        value.Should().Be(18446744073709551615);
    }
}
