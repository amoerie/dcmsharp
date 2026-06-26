using FellowOakDicom;
using Xunit;

namespace DcmOrganize.Tests;

public class TestsForDicomTagParser
{
    private readonly DicomTagParser _dicomTagParser;

    public TestsForDicomTagParser()
    {
        _dicomTagParser = new DicomTagParser();
    }

    [Fact]
    public void ShouldParseTagByGroupAndElement()
    {
        var dicomTag = _dicomTagParser.Parse("(0008,0050)");
        Assert.Equal(DicomTag.AccessionNumber, dicomTag);
    }

    [Fact]
    public void ShouldParseTagByName()
    {
        var dicomTag = _dicomTagParser.Parse("AccessionNumber");
        Assert.Equal(DicomTag.AccessionNumber, dicomTag);
    }

    [Fact]
    public void ShouldNotParseUnknownTag()
    {
        Assert.Throws<DicomTagParserException>(() => _dicomTagParser.Parse("Banana"));
    }
}
