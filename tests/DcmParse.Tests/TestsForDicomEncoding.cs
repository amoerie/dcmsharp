using FluentAssertions;
using Xunit.Abstractions;

namespace DcmParse.Tests;

[Collection(nameof(DicomParserCollection))]
public sealed class TestsForDicomEncoding
{
    private readonly IDicomParser _dicomParser;

    public TestsForDicomEncoding(DicomParserFixture fixture, ITestOutputHelper output)
    {
        fixture.OutputHelper = output;
        _dicomParser = fixture.DicomParser;
    }

    [Fact]
    public async Task ShouldParseEncodedPatientName()
    {
        // Arrange
        var file = new FileInfo("./Dicom/Encoded.dcm");
        using var dicomDataset = await _dicomParser.ParseAsync(file);

        // Act
        dicomDataset.TryGetPersonName(DicomTags.PatientName, out DicomPersonName patientName).Should().BeTrue();

        // Assert
        patientName.GivenName.Should().Be("Härold");
        patientName.FamilyName.Should().Be("Å Кириллица");
    }

}
