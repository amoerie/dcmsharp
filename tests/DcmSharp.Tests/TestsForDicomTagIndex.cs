using System.Reflection;

namespace DcmSharp.Tests;

public class TestsForDicomTagIndex
{
    public class TestsForTryLookup : TestsForDicomTagIndex
    {
        [Theory]
        [InlineData(0x0008, 0x0018)]
        [InlineData(0x0010, 0x0010)]
        public void ShouldReturnTrueAndCorrectTagWhenGivenValidTag(ushort group, ushort element)
        {
            // Arrange & Act
            bool result = DicomTagsIndex.TryLookup(group, element, out DicomTag? tag);

            // Assert
            Assert.True(result);
            Assert.Equal(group, tag!.Group);
            Assert.Equal(element, tag.Element);
        }

        [Fact]
        public void ShouldReturnFalseWhenGivenUnknownTag()
        {
            // Arrange & Act
            bool result = DicomTagsIndex.TryLookup(0xFFFF, 0xFFFF, out DicomTag? _);

            // Assert
            Assert.False(result);
        }
    }

    public class TestsForTryLookupByName : TestsForDicomTagIndex
    {
        public static readonly IEnumerable<object[]> DicomTagNames = typeof(DicomTags)
            .GetFields(BindingFlags.Static | BindingFlags.Public)
            .Where(f => f.FieldType == typeof(DicomTag))
            .Select(f => (object[])[f.Name])
            .ToArray();

        [Theory]
        [MemberData(nameof(DicomTagNames))]
        public void ShouldReturnTrueAndCorrectTagWhenGivenValidTag(string dicomTagName)
        {
            // Arrange & Act
            bool result = DicomTagsIndex.TryLookupByKeyword(dicomTagName, out DicomTag? tag);

            // Assert
            Assert.True(result);
            Assert.NotNull(tag);
        }

        [Fact]
        public void ShouldReturnFalseWhenGivenUnknownTag()
        {
            // Arrange & Act
            bool result = DicomTagsIndex.TryLookupByKeyword(
                "VortexPhaseScintillator",
                out DicomTag? _
            );

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldReturnFalseWhenGivenEmptyString()
        {
            // Arrange & Act
            bool result = DicomTagsIndex.TryLookupByKeyword("", out DicomTag? _);

            // Assert
            Assert.False(result);
        }
    }
}
