using System.Text;
using DcmSharp.Parser;
using FluentAssertions;
using Xunit.Abstractions;

namespace DcmSharp.Tests;

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
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(file);

        // Act
        dicomDataset.TryGetString(DicomTags.SpecificCharacterSet, out string? specificCharacterSet).Should().BeTrue();
        dicomDataset.TryGetPersonName(DicomTags.PatientName, out PersonName patientName).Should().BeTrue();
        dicomDataset.TryGetString(DicomTags.PatientName, out string? patientNameString).Should().BeTrue();

        // Assert
        DicomEncoding.TryParse(specificCharacterSet!, out var encoding).Should().BeTrue();
        encoding.Should().Be(Encoding.Latin1);
        patientName.GivenName.Should().Be("Jørgen");
        patientName.FamilyName.Should().Be("Åseline");
        patientNameString.Should().Be("Åseline^Jørgen");
    }

}
