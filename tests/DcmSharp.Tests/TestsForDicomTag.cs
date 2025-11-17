namespace DcmSharp.Tests;

public class TestsForDicomTag
{
    public class TestsForTryParse: TestsForDicomTag
    {
        [Theory]
        [InlineData("(0010,0010)", 0x0010, 0x0010)]
        [InlineData("0010,0010", 0x0010, 0x0010)]
        [InlineData("00100010", 0x0010, 0x0010)]
        public void ShouldReturnTrueAndCorrectTagWhenGivenValidTag(string tagString, ushort expectedGroup, ushort expectedElement)
        {
            // Arrange & Act
            bool result = DicomTag.TryParse(tagString, out DicomTag? tag);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedGroup, tag!.Group);
            Assert.Equal(expectedElement, tag.Element);
        }

        [Fact]
        public void ShouldReturnFalseWhenGivenInvalidInput()
        {
            // Arrange & Act
            bool result = DicomTag.TryParse("InvalidTag", out DicomTag? _);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldReturnFalseWhenGivenUnknownTag()
        {
            // Arrange & Act
            bool result = DicomTag.TryParse("(FFFF,FFFF)", out DicomTag? _);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldReturnFalseWhenGivenEmptyString()
        {
            // Arrange & Act
            bool result = DicomTag.TryParse("", out DicomTag? _);

            // Assert
            Assert.False(result);
        }
    }
}
