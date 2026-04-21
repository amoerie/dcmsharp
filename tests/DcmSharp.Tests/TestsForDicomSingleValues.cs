using DcmSharp.Parser;

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
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetString(DicomTags.SelectorAEValue, out string? value));

        // Assert
        Assert.Equal("MODALITY1", value);
    }

    [Fact]
    public async Task ShouldParseAS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetString(DicomTags.SelectorASValue, out string? value));

        // Assert
        Assert.Equal("025Y", value);
    }

    [Fact]
    public async Task ShouldParseAT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetTag(DicomTags.SelectorATValue, out DicomTag? value));

        // Assert
        Assert.Equal(DicomTags.TransferSyntaxUID, value);
    }

    [Fact]
    public async Task ShouldParseDA()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetDate(DicomTags.SelectorDAValue, out DateOnly value));

        // Assert
        Assert.Equal(DateOnly.Parse("2024-12-03"), value);
    }

    [Fact]
    public async Task ShouldParseDS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetDecimal(DicomTags.SelectorDSValue, out decimal value));

        // Assert
        Assert.Equal(0.25m, value);
    }

    [Fact]
    public async Task ShouldParseDT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetDateTime(DicomTags.SelectorDTValue, out DateTime value));

        // Assert
        Assert.Equal(DateTime.Parse("2024-12-03T12:00:00"), value);
    }

    [Fact]
    public async Task ShouldParseFD()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetDouble(DicomTags.SelectorFDValue, out double value));

        // Assert
        Assert.Equal(-100.123, value);
    }

    [Fact]
    public async Task ShouldParseFL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetFloat(DicomTags.SelectorFLValue, out float value));

        // Assert
        Assert.Equal(100.5f, value);
    }

    [Fact]
    public async Task ShouldParseIS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetInt(DicomTags.SelectorISValue, out int value));

        // Assert
        Assert.Equal(1, value);
    }

    [Fact]
    public async Task ShouldParseLO()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetString(DicomTags.SelectorLOValue, out string? value));

        // Assert
        Assert.Equal("Medical Center A", value);
    }

    [Fact]
    public async Task ShouldParseLT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetString(DicomTags.SelectorLTValue, out string? value));

        // Assert
        Assert.Equal("Some long notes", value);
    }

    [Fact]
    public async Task ShouldParsePN()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetPersonName(DicomTags.SelectorPNValue, out PersonName value));

        // Assert
        var expected = new PersonName("Dr", "Smith", null, null, null, null, null, null, null);
        Assert.Equal(expected, value);
    }

    [Fact]
    public async Task ShouldParseSH()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetString(DicomTags.SelectorSHValue, out string? value));

        // Assert
        Assert.Equal("CT123", value);
    }

    [Fact]
    public async Task ShouldParseSL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetInt(DicomTags.SelectorSLValue, out int value));

        // Assert
        Assert.Equal(-1, value);
    }

    [Fact]
    public async Task ShouldParseSS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetShort(DicomTags.SelectorSSValue, out short value));

        // Assert
        Assert.Equal(-32768, value);
    }

    [Fact]
    public async Task ShouldParseST()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetString(DicomTags.SelectorSTValue, out string? value));

        // Assert
        Assert.Equal("History1", value);
    }

    [Fact]
    public async Task ShouldParseSV()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetLong(DicomTags.SelectorSVValue, out long value));

        // Assert
        Assert.Equal(9223372036854775807, value);
    }

    [Fact]
    public async Task ShouldParseTM()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetTime(DicomTags.SelectorTMValue, out TimeOnly value));

        // Assert
        Assert.Equal(TimeOnly.Parse("12:00:00"), value);
    }

    [Fact]
    public async Task ShouldParseUC()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetString(DicomTags.SelectorUCValue, out string? value));

        // Assert
        Assert.Equal("Device A", value);
    }

    [Fact]
    public async Task ShouldParseUI()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetString(DicomTags.SelectorUIValue, out string? value));

        // Assert
        Assert.Equal("1.2.3", value);
    }

    [Fact]
    public async Task ShouldParseUL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetUInt(DicomTags.SelectorULValue, out uint value));

        // Assert
        Assert.Equal(4294967295, value);
    }

    [Fact]
    public async Task ShouldParseUS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetUShort(DicomTags.SelectorUSValue, out ushort value));

        // Assert
        Assert.Equal(1, value);
    }

    [Fact]
    public async Task ShouldParseUT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetString(DicomTags.SelectorUTValue, out string? value));

        // Assert
        Assert.Equal("Modified", value);
    }

    [Fact]
    public async Task ShouldParseUV()
    {
        // Arrange
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetULong(DicomTags.SelectorUVValue, out ulong value));

        // Assert
        Assert.Equal(18446744073709551615, value);
    }
}
