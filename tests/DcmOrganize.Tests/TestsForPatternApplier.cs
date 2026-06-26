using FellowOakDicom;
using Xunit;

namespace DcmOrganize.Tests;

public class TestsForPatternApplier
{
    private readonly PatternApplier _patternApplier;

    public TestsForPatternApplier()
    {
        var dicomTagParser = new DicomTagParser();
        var folderNameCleaner = new FolderNameCleaner();
        _patternApplier = new PatternApplier(dicomTagParser, folderNameCleaner);
    }

    [Fact]
    public void ShouldApplySimplePattern()
    {
        // Arrange
        var dicomDataSet = new DicomDataset
        {
            { DicomTag.AccessionNumber, "ABC123" },
            { DicomTag.InstanceNumber, "7" },
        };
        var pattern = "{AccessionNumber}/{InstanceNumber}.dcm";

        // Act
        var file = _patternApplier.Apply(dicomDataSet, pattern);

        // Assert
        Assert.Equal(Path.Join("ABC123", "7.dcm"), file);
    }

    [Fact]
    public void ShouldApplyComplexPattern()
    {
        // Arrange
        var dicomDataSet = new DicomDataset
        {
            { DicomTag.PatientName, "Samson^Gert" },
            { DicomTag.AccessionNumber, "ABC123" },
            { DicomTag.SeriesNumber, "20" },
            { DicomTag.InstanceNumber, "7" },
        };
        var pattern =
            "Patient {PatientName}/Study {AccessionNumber}/Series {SeriesNumber}/Image {InstanceNumber}.dcm";

        // Act
        var file = _patternApplier.Apply(dicomDataSet, pattern);

        // Assert
        Assert.Equal(
            Path.Join("Patient Samson Gert", "Study ABC123", "Series 20", "Image 7.dcm"),
            file
        );
    }

    [Fact]
    public void ShouldUseValueWhenPatternContainsFallbackAndValueIsPresent()
    {
        // Arrange
        var dicomDataSet = new DicomDataset
        {
            { DicomTag.SOPInstanceUID, "1.2.3" },
            { DicomTag.InstanceNumber, "10" },
        };
        var pattern = "{InstanceNumber ?? SOPInstanceUID}.dcm";

        // Act
        var file = _patternApplier.Apply(dicomDataSet, pattern);

        // Assert
        Assert.Equal("10.dcm", file);
    }

    [Fact]
    public void ShouldUseFallbackWhenPatternContainsFallbackAndValueIsNotPresent()
    {
        // Arrange
        var dicomDataSet = new DicomDataset { { DicomTag.SOPInstanceUID, "1.2.3" } };
        var pattern = "{InstanceNumber ?? SOPInstanceUID}.dcm";

        // Act
        var file = _patternApplier.Apply(dicomDataSet, pattern);

        // Assert
        Assert.Equal("1.2.3.dcm", file);
    }

    [Fact]
    public void ShouldSupportGuidsInFilePattern()
    {
        // Arrange
        var dicomDataSet = new DicomDataset { { DicomTag.SOPInstanceUID, "1.2.3" } };
        var pattern = "{Guid}.dcm";

        // Act
        var file = _patternApplier.Apply(dicomDataSet, pattern);

        // Assert
        var guidAsString = file!.Substring(0, file.Length - ".dcm".Length);

        Assert.True(Guid.TryParse(guidAsString, out var _));
    }

    [Fact]
    public void ShouldThrowExceptionWhenAnErrorOccurs()
    {
        // Arrange
        var dicomDataSet = new DicomDataset { { DicomTag.SOPInstanceUID, "1.2.3" } };
        var pattern = "{Banana}.dcm";

        // Act
        Assert.Throws<PatternException>(() => _patternApplier.Apply(dicomDataSet, pattern));
    }

    [Fact]
    public void ShouldSupportConstantsAsFallback()
    {
        // Arrange
        var dicomDataSet = new DicomDataset { { DicomTag.SOPInstanceUID, "1.2.3" } };
        var pattern = "{InstanceNumber ?? 'Constant'}.dcm";

        // Act
        Assert.Equal("Constant.dcm", _patternApplier.Apply(dicomDataSet, pattern));
    }
}
