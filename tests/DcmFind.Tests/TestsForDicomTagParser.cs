using DcmSharp;
using Xunit;

namespace DcmFind.Tests;

[Collection("DcmFind")]
public class TestsForDicomTagParser
{
    [Fact]
    public void ShouldParseTagByGroupAndElement()
    {
        Assert.True(DicomTagParser.TryParse("(0008,0050)", out var dicomTag));
        Assert.Equal(DicomTags.AccessionNumber, dicomTag);
    }

    [Fact]
    public void ShouldParseTagByName()
    {
        Assert.True(DicomTagParser.TryParse("AccessionNumber", out var dicomTag));
        Assert.Equal(DicomTags.AccessionNumber, dicomTag);
    }

    [Fact]
    public void ShouldNotParseUnknownTag()
    {
        Assert.False(DicomTagParser.TryParse("Banana", out _));
    }
}
