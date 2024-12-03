using DcmSharp.Parser;
using FluentAssertions;
using Xunit.Abstractions;

namespace DcmSharp.Tests;

[Collection(nameof(DicomParserCollection))]
public sealed class TestsForDicomMultiValues
{
    private readonly IDicomParser _dicomParser;

    public TestsForDicomMultiValues(DicomParserFixture fixture, ITestOutputHelper output)
    {
        fixture.OutputHelper = output;
        _dicomParser = fixture.DicomParser;
    }

    [Fact]
    public async Task ShouldParseAE()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetStrings(DicomTags.SelectorAEValue, out string[]? values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo(["MODALITY1", "MODALITY2"]);
    }

    [Fact]
    public async Task ShouldParseAS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetStrings(DicomTags.SelectorASValue, out string[]? values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo(["025Y", "030D"]);
    }

    [Fact]
    public async Task ShouldParseAT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetTags(DicomTags.SelectorATValue, out DicomTag[]? values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([DicomTags.TransferSyntaxUID, DicomTags.ImplementationClassUID]);
    }

    [Fact]
    public async Task ShouldParseDA()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetDates(DicomTags.SelectorDAValue, out DateOnly[]? values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([DateOnly.Parse("2024-12-03"), DateOnly.Parse("2024-11-03")]);
    }

    [Fact]
    public async Task ShouldParseDS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetDecimals(DicomTags.SelectorDSValue, out decimal[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([0.25m, 0.50m]);
    }

    [Fact]
    public async Task ShouldParseDT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetDateTimes(DicomTags.SelectorDTValue, out DateTime[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([DateTime.Parse("2024-12-03T12:00:00"), DateTime.Parse("2024-11-03T12:00:00")]);
    }

    [Fact]
    public async Task ShouldParseFD()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetDoubles(DicomTags.SelectorFDValue, out double[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([-100.123, 200.456]);
    }

    [Fact]
    public async Task ShouldParseFL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetFloats(DicomTags.SelectorFLValue, out float[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([100.5f, 200.5]);
    }

    [Fact]
    public async Task ShouldParseIS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetInts(DicomTags.SelectorISValue, out int[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([1, 2]);
    }

    [Fact]
    public async Task ShouldParseLO()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetStrings(DicomTags.SelectorLOValue, out string[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo(["Medical Center A", "Medical Center B"]);
    }

    [Fact]
    public async Task ShouldParseLT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetStrings(DicomTags.SelectorLTValue, out string[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo(["Some long notes"]);
    }

    [Fact]
    public async Task ShouldParsePN()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetPersonNames(DicomTags.SelectorPNValue, out DicomPersonName[] values).Should().BeTrue();

        // Assert
        var expected1 = new DicomPersonName("Dr", "Smith", null, null, null, null, null, null, null);
        var expected2 = new DicomPersonName("Dr", "Jones", null, null, null, null, null, null, null);
        values.Should().BeEquivalentTo([expected1, expected2]);
    }

    [Fact]
    public async Task ShouldParseSH()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetStrings(DicomTags.SelectorSHValue, out string[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([ "CT123", "CT456"]);
    }

    [Fact]
    public async Task ShouldParseSL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetInts(DicomTags.SelectorSLValue, out int[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([-1, 2]);
    }

    [Fact]
    public async Task ShouldParseSS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetShorts(DicomTags.SelectorSSValue, out short[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([-32768, 32767]);
    }

    [Fact]
    public async Task ShouldParseST()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetStrings(DicomTags.SelectorSTValue, out string[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([ "History1"]);
    }

    [Fact]
    public async Task ShouldParseSV()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetLongs(DicomTags.SelectorSVValue, out long[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([9223372036854775807, -9223372036854775808]);
    }

    [Fact]
    public async Task ShouldParseTM()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetTimes(DicomTags.SelectorTMValue, out TimeOnly[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([TimeOnly.Parse("12:00:00"), TimeOnly.Parse("13:00:00")]);
    }

    [Fact]
    public async Task ShouldParseUC()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetStrings(DicomTags.SelectorUCValue, out string[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo(["Device A", "Device B"]);
    }

    [Fact]
    public async Task ShouldParseUI()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetStrings(DicomTags.SelectorUIValue, out string[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo(["1.2.3", "4.5.6"]);
    }

    [Fact]
    public async Task ShouldParseUL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetUInts(DicomTags.SelectorULValue, out uint[] values).Should().BeTrue();

        // Assert
        List<uint> expected = [4294967295, 2];
        values.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ShouldParseUS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetUShorts(DicomTags.SelectorUSValue, out ushort[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([1, 100]);
    }

    [Fact]
    public async Task ShouldParseUT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetStrings(DicomTags.SelectorUTValue, out string[] values).Should().BeTrue();

        // Assert
        values.Should().BeEquivalentTo([ "Modified" ]);
    }

    [Fact]
    public async Task ShouldParseUV()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetULongs(DicomTags.SelectorUVValue, out ulong[] values).Should().BeTrue();

        // Assert
        List<ulong> expected = [18446744073709551615, 18446744073709551614];
        values.Should().BeEquivalentTo(expected);
    }
}
