using DcmSharp;
using Xunit;

namespace DcmFind.Tests;

[Collection("DcmFind")]
public class TestsForQuery
{
    private readonly DicomDataset _dicomDataset;

    public TestsForQuery()
    {
        _dicomDataset = new DicomDataset();
        _dicomDataset.Add(DicomTags.AccessionNumber, "Pineapple");
        _dicomDataset.Add(DicomTags.Rows, 1000);
        _dicomDataset.Add(DicomTags.StudyDate, new DateOnly(2020, 1, 2));
    }

    public class TestsForEqualsQuery : TestsForQuery
    {
        [Fact]
        public void ShouldMatchWhenValuesAreEqual()
        {
            var query = new EqualsQuery(DicomTags.AccessionNumber, "Pineapple");

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldNotMatchWhenValuesAreNotEqual()
        {
            var query = new EqualsQuery(DicomTags.AccessionNumber, "Pineapple2");

            Assert.False(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldNotMatchWhenDicomTagIsNotPresent()
        {
            var query = new EqualsQuery(DicomTags.AccessionNumber, "Pineapple");

            _dicomDataset.Clear();

            Assert.False(query.Matches(_dicomDataset));
        }

        [Theory]
        [InlineData("%apple")]
        [InlineData("%Apple")]
        [InlineData("Pine%")]
        [InlineData("pine%")]
        [InlineData("Pine%apple")]
        public void ShouldMatchWhenValuesMatchesWildcardInBeginning(string value)
        {
            var query = new EqualsQuery(DicomTags.AccessionNumber, value);

            Assert.True(query.Matches(_dicomDataset));
        }
    }

    public class TestsForNotEqualsQuery : TestsForQuery
    {
        [Fact]
        public void ShouldNotMatchWhenValuesAreEqual()
        {
            var query = new NotEqualsQuery(DicomTags.AccessionNumber, "Pineapple");

            Assert.False(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenValuesAreNotEqual()
        {
            var query = new NotEqualsQuery(DicomTags.AccessionNumber, "Pineapple2");

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenDicomTagIsNotPresent()
        {
            var query = new NotEqualsQuery(DicomTags.AccessionNumber, "Pineapple");

            _dicomDataset.Clear();

            Assert.True(query.Matches(_dicomDataset));
        }

        [Theory]
        [InlineData("%apple")]
        [InlineData("%Apple")]
        [InlineData("Pine%")]
        [InlineData("pine%")]
        [InlineData("Pine%apple")]
        public void ShouldNotMatchWhenValuesMatchesWildcardInBeginning(string value)
        {
            var query = new NotEqualsQuery(DicomTags.AccessionNumber, value);

            Assert.False(query.Matches(_dicomDataset));
        }

        [Theory]
        [InlineData("%banana")]
        [InlineData("%Banana")]
        [InlineData("Banana%")]
        [InlineData("bana%")]
        [InlineData("Bana%na")]
        public void ShouldMatchWhenValuesDoesNotMatchWildcard(string value)
        {
            var query = new NotEqualsQuery(DicomTags.AccessionNumber, value);

            Assert.True(query.Matches(_dicomDataset));
        }
    }

    public class TestsForLowerThanQuery : TestsForQuery
    {
        [Fact]
        public void ShouldMatchWhenNumberValueIsLowerThanNotInclusive()
        {
            var query = new LowerThanQuery(DicomTags.Rows, "1001", false);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenNumberValueIsLowerThanInclusive()
        {
            var query = new LowerThanQuery(DicomTags.Rows, "1001", true);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldNotMatchWhenNumberValueIsEqualAndNotInclusive()
        {
            var query = new LowerThanQuery(DicomTags.Rows, "1000", false);

            Assert.False(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenNumberValueIsEqualAndInclusive()
        {
            var query = new LowerThanQuery(DicomTags.Rows, "1000", true);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenStringValueIsLowerThanNotInclusive()
        {
            var query = new LowerThanQuery(DicomTags.AccessionNumber, "Pineapplf", false);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenStringValueIsLowerThanInclusive()
        {
            var query = new LowerThanQuery(DicomTags.AccessionNumber, "Pineapplf", true);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldNotMatchWhenStringValueIsEqualAndNotInclusive()
        {
            var query = new LowerThanQuery(DicomTags.AccessionNumber, "Pineapple", false);

            Assert.False(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenStringValueIsEqualAndInclusive()
        {
            var query = new LowerThanQuery(DicomTags.AccessionNumber, "Pineapple", true);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenDateValueIsLowerThanNotInclusive()
        {
            var query = new LowerThanQuery(DicomTags.StudyDate, "20200103", false);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenDateValueIsLowerThanInclusive()
        {
            var query = new LowerThanQuery(DicomTags.StudyDate, "20200103", true);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldNotMatchWhenDateValueIsEqualAndNotInclusive()
        {
            var query = new LowerThanQuery(DicomTags.StudyDate, "20200102", false);

            Assert.False(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenDateValueIsEqualAndInclusive()
        {
            var query = new LowerThanQuery(DicomTags.StudyDate, "20200102", true);

            Assert.True(query.Matches(_dicomDataset));
        }
    }

    public class TestsForGreaterThanQuery : TestsForQuery
    {
        [Fact]
        public void ShouldMatchWhenNumberValueIsGreaterThanNotInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.Rows, "999", false);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenNumberValueIsGreaterThanInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.Rows, "999", true);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldNotMatchWhenNumberValueIsEqualAndNotInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.Rows, "1000", false);

            Assert.False(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenNumberValueIsEqualAndInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.Rows, "1000", true);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenStringValueIsGreaterThanNotInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.AccessionNumber, "Pineappld", false);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenStringValueIsGreaterThanInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.AccessionNumber, "Pineappld", true);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldNotMatchWhenStringValueIsEqualAndNotInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.AccessionNumber, "Pineapple", false);

            Assert.False(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenStringValueIsEqualAndInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.AccessionNumber, "Pineapple", true);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenDateValueIsGreaterThanNotInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.StudyDate, "20200101", false);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenDateValueIsGreaterThanInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.StudyDate, "20200101", true);

            Assert.True(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldNotMatchWhenDateValueIsEqualAndNotInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.StudyDate, "20200102", false);

            Assert.False(query.Matches(_dicomDataset));
        }

        [Fact]
        public void ShouldMatchWhenDateValueIsEqualAndInclusive()
        {
            var query = new GreaterThanQuery(DicomTags.StudyDate, "20200102", true);

            Assert.True(query.Matches(_dicomDataset));
        }
    }
}
