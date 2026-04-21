using DcmSharp.Parser;

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
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetStrings(DicomTags.SelectorAEValue, out string[]? values));

        // Assert
        Assert.Equivalent(new[] { "MODALITY1", "MODALITY2" }, values);
    }

    [Fact]
    public async Task ShouldParseAS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetStrings(DicomTags.SelectorASValue, out string[]? values));

        // Assert
        Assert.Equivalent(new[] { "025Y", "030D" }, values);
    }

    [Fact]
    public async Task ShouldParseAT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetTags(DicomTags.SelectorATValue, out DicomTag[]? values));

        // Assert
        Assert.Equivalent(
            new[] { DicomTags.TransferSyntaxUID, DicomTags.ImplementationClassUID },
            values
        );
    }

    [Fact]
    public async Task ShouldParseDA()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetDates(DicomTags.SelectorDAValue, out DateOnly[]? values));

        // Assert
        Assert.Equivalent(
            new[] { DateOnly.Parse("2024-12-03"), DateOnly.Parse("2024-11-03") },
            values
        );
    }

    [Fact]
    public async Task ShouldParseDS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetDecimals(DicomTags.SelectorDSValue, out decimal[] values));

        // Assert
        Assert.Equivalent(new[] { 0.25m, 0.50m }, values);
    }

    [Fact]
    public async Task ShouldParseDT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetDateTimes(DicomTags.SelectorDTValue, out DateTime[] values));

        // Assert
        Assert.Equivalent(
            new[] { DateTime.Parse("2024-12-03T12:00:00"), DateTime.Parse("2024-11-03T12:00:00") },
            values
        );
    }

    [Fact]
    public async Task ShouldParseFD()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetDoubles(DicomTags.SelectorFDValue, out double[] values));

        // Assert
        Assert.Equivalent(new[] { -100.123, 200.456 }, values);
    }

    [Fact]
    public async Task ShouldParseFL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetFloats(DicomTags.SelectorFLValue, out float[] values));

        // Assert
        Assert.Equivalent(new[] { 100.5f, 200.5f }, values);
    }

    [Fact]
    public async Task ShouldParseIS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetInts(DicomTags.SelectorISValue, out int[] values));

        // Assert
        Assert.Equivalent(new[] { 1, 2 }, values);
    }

    [Fact]
    public async Task ShouldParseLO()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetStrings(DicomTags.SelectorLOValue, out string[] values));

        // Assert
        Assert.Equivalent(new[] { "Medical Center A", "Medical Center B" }, values);
    }

    [Fact]
    public async Task ShouldParseLT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetStrings(DicomTags.SelectorLTValue, out string[] values));

        // Assert
        Assert.Equivalent(new[] { "Some long notes" }, values);
    }

    [Fact]
    public async Task ShouldParsePN()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(
            dicomDataset.TryGetPersonNames(DicomTags.SelectorPNValue, out PersonName[] values)
        );

        // Assert
        var expected1 = new PersonName("Dr", "Smith", null, null, null, null, null, null, null);
        var expected2 = new PersonName("Dr", "Jones", null, null, null, null, null, null, null);
        Assert.Equivalent(new[] { expected1, expected2 }, values);
    }

    [Fact]
    public async Task ShouldParseSH()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetStrings(DicomTags.SelectorSHValue, out string[] values));

        // Assert
        Assert.Equivalent(new[] { "CT123", "CT456" }, values);
    }

    [Fact]
    public async Task ShouldParseSL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetInts(DicomTags.SelectorSLValue, out int[] values));

        // Assert
        Assert.Equivalent(new[] { -1, 2 }, values);
    }

    [Fact]
    public async Task ShouldParseSS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetShorts(DicomTags.SelectorSSValue, out short[] values));

        // Assert
        Assert.Equivalent(new short[] { -32768, 32767 }, values);
    }

    [Fact]
    public async Task ShouldParseST()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetStrings(DicomTags.SelectorSTValue, out string[] values));

        // Assert
        Assert.Equivalent(new[] { "History1" }, values);
    }

    [Fact]
    public async Task ShouldParseSV()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetLongs(DicomTags.SelectorSVValue, out long[] values));

        // Assert
        Assert.Equivalent(new[] { 9223372036854775807, -9223372036854775808 }, values);
    }

    [Fact]
    public async Task ShouldParseTM()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetTimes(DicomTags.SelectorTMValue, out TimeOnly[] values));

        // Assert
        Assert.Equivalent(new[] { TimeOnly.Parse("12:00:00"), TimeOnly.Parse("13:00:00") }, values);
    }

    [Fact]
    public async Task ShouldParseUC()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetStrings(DicomTags.SelectorUCValue, out string[] values));

        // Assert
        Assert.Equivalent(new[] { "Device A", "Device B" }, values);
    }

    [Fact]
    public async Task ShouldParseUI()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetStrings(DicomTags.SelectorUIValue, out string[] values));

        // Assert
        Assert.Equivalent(new[] { "1.2.3", "4.5.6" }, values);
    }

    [Fact]
    public async Task ShouldParseUL()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetUInts(DicomTags.SelectorULValue, out uint[] values));

        // Assert
        Assert.Equivalent(new uint[] { 4294967295, 2 }, values);
    }

    [Fact]
    public async Task ShouldParseUS()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetUShorts(DicomTags.SelectorUSValue, out ushort[] values));

        // Assert
        Assert.Equivalent(new ushort[] { 1, 100 }, values);
    }

    [Fact]
    public async Task ShouldParseUT()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetStrings(DicomTags.SelectorUTValue, out string[] values));

        // Assert
        Assert.Equivalent(new[] { "Modified" }, values);
    }

    [Fact]
    public async Task ShouldParseUV()
    {
        // Arrange
        var file = new FileInfo("./Dicom/MultiValues.dcm");
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(dicomDataset.TryGetULongs(DicomTags.SelectorUVValue, out ulong[] values));

        // Assert
        Assert.Equivalent(new ulong[] { 18446744073709551615, 18446744073709551614 }, values);
    }
}
