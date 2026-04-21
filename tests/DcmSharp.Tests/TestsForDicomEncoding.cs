using System.Text;
using DcmSharp.Parser;

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
        using var dicomDataset = await _dicomParser.ParseReadOnlyAsync(
            file,
            TestContext.Current.CancellationToken
        );

        // Act
        Assert.True(
            dicomDataset.TryGetString(
                DicomTags.SpecificCharacterSet,
                out string? specificCharacterSet
            )
        );
        Assert.True(
            dicomDataset.TryGetPersonName(DicomTags.PatientName, out PersonName patientName)
        );
        Assert.True(
            dicomDataset.TryGetString(DicomTags.PatientName, out string? patientNameString)
        );

        // Assert
        Assert.True(DicomEncoding.TryParse(specificCharacterSet!, out var encoding));
        Assert.Equal(Encoding.Latin1, encoding);
        Assert.Equal("Jørgen", patientName.GivenName);
        Assert.Equal("Åseline", patientName.FamilyName);
        Assert.Equal("Åseline^Jørgen", patientNameString);
    }
}
