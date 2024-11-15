using System.Text;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DcmParse.Tests;

public sealed class TestsForDicomParser : IDisposable
{
    private readonly ServiceProvider _services;
    private readonly DicomParser _dicomParser;

    public TestsForDicomParser(ITestOutputHelper output)
    {
        _services = new ServiceCollection()
            .AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddXUnit(output);
            })
            .AddSingleton<DicomParser>()
            .BuildServiceProvider();
        _dicomParser = _services.GetRequiredService<DicomParser>();
    }

    public void Dispose()
    {
        _services.Dispose();
    }

    [Fact]
    public async Task ShouldParseExplicitValueRepresentation()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");

        // Act + Assert
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Assert
        dicomDataset.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldParseImplicitValueRepresentation()
    {
        // Arrange
        var file = new FileInfo("./Dicom/ImplicitVR.dcm");

        // Act + Assert
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Assert
        dicomDataset.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0x0008, 0x0018, "2.25.332838821141227624838581964210008219211")]
    [InlineData(0x0040, 0x2016, "ORDER2024081216321")]
    public async Task ShouldRetrieveDicomTagFromExplicitVRDataset(ushort group, ushort element, string expectedValue)
    {
        // Arrange
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetRaw(group, element, out ReadOnlyMemory<byte>? rawValue).Should().BeTrue();

        // Assert
        string actualValue = Encoding.ASCII.GetString(rawValue!.Value.Span);
        actualValue.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(0x0008, 0x0018, "1.2.840.113619.2.1.2411.1031152382.365.1.736169244")]
    [InlineData(0x0028, 0x1054, "US")]
    public async Task ShouldRetrieveDicomTagFromImplicitVRDataset(ushort group, ushort element, string expectedValue)
    {
        // Arrange
        var file = new FileInfo("./Dicom/ImplicitVR.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetRaw(group, element, out ReadOnlyMemory<byte>? rawValue).Should().BeTrue();

        // Assert
        string actualValue = Encoding.ASCII.GetString(rawValue!.Value.Span);
        actualValue.Should().Be(expectedValue);
    }

    [Fact]
    public async Task ShouldRetrieveDicomTagFromNestedSequence()
    {
        // Arrange
        var file = new FileInfo("./Dicom/TestPatternPalette.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act + Assert
        dicomDataset.TryGetSequence(DicomTags.SourceImageSequence, out IReadOnlyList<DicomDataset>? sourceImageSequence)
            .Should().BeTrue();
        sourceImageSequence.Should().NotBeNull();
        var firstSourceImage = sourceImageSequence!.First();
        firstSourceImage.Should().NotBeNull();
        firstSourceImage.TryGetSequence(DicomTags.PurposeOfReferenceCodeSequence,
                out IReadOnlyList<DicomDataset>? purposeOfReferenceCodeSequence)
            .Should().BeTrue();
        purposeOfReferenceCodeSequence.Should().NotBeNull();
        var firstPurposeOfReferenceCodeSequence = purposeOfReferenceCodeSequence!.First();
        firstPurposeOfReferenceCodeSequence.TryGetRaw(DicomTags.CodeMeaning, out ReadOnlyMemory<byte>? codeMeaningValue).Should().BeTrue();
        codeMeaningValue.Should().NotBeNull();
        string codeMeaning = Encoding.ASCII.GetString(codeMeaningValue!.Value.Span);
        codeMeaning.Should().Be("Uncompressed predecessor");
    }

    // TODO: Add support for encoding based on DicomTag Specific Character Set
}
