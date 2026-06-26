using Xunit;

namespace DcmFind.Tests;

[Collection("DcmFind")]
public class TestsForQueryParser
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldReturnFalseWhenQueryValueIsNullOrEmpty(string? queryValue)
    {
        Assert.False(QueryParser.TryParse(queryValue, out _));
    }

    [Fact]
    public void ShouldReturnFalseWhenOperatorIsMissing()
    {
        Assert.False(QueryParser.TryParse("AccessionNumberPineapple", out _));
    }

    [Fact]
    public void ShouldReturnFalseWhenOperatorIsUnknown()
    {
        Assert.False(QueryParser.TryParse("AccessionNumber~Pineapple", out _));
    }

    [Fact]
    public void ShouldReturnFalseWhenDicomTagIsUnknown()
    {
        Assert.False(QueryParser.TryParse("Banana=Pineapple", out _));
    }

    [Fact]
    public void ShouldReturnEqualsQuery()
    {
        Assert.True(QueryParser.TryParse("AccessionNumber=Pineapple", out var query));
        Assert.True((query) is EqualsQuery);
    }

    [Fact]
    public void ShouldReturnEqualsQueryEvenIfQueryValueContainsLowerThan()
    {
        Assert.True(QueryParser.TryParse("AccessionNumber=Pineapple<=3", out var query));
        Assert.True((query) is EqualsQuery);
    }

    [Fact]
    public void ShouldReturnLowerThanQuery()
    {
        Assert.True(QueryParser.TryParse("AccessionNumber<=Pineapple", out var query));
        Assert.True((query) is LowerThanQuery);
    }

    [Fact]
    public void ShouldReturnLowerThanQueryEvenIfQueryValueContainsEquals()
    {
        Assert.True(QueryParser.TryParse("AccessionNumber<=Pineapple=3", out var query));
        Assert.True((query) is LowerThanQuery);
    }

    [Fact]
    public void ShouldReturnLowerThanOrEqualsQuery()
    {
        Assert.True(QueryParser.TryParse("AccessionNumber<Pineapple", out var query));
        Assert.True((query) is LowerThanQuery);
    }

    [Fact]
    public void ShouldReturnNotEqualsQuery()
    {
        Assert.True(QueryParser.TryParse("AccessionNumber!=\"\"", out var query));
        Assert.True((query) is NotEqualsQuery);
    }
}
