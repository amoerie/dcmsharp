using DcmAnonymize.Patient;
using Xunit;

namespace DcmAnonymize.Tests.Patient;

[Collection("DcmAnonymize")]
public class TestsForNationalNumberGenerator
{
    private readonly NationalNumberGenerator _nationalNumberGenerator;

    public TestsForNationalNumberGenerator()
    {
        _nationalNumberGenerator = new NationalNumberGenerator();
    }

    [Fact]
    public void ShouldGenerateCorrectMaleNationalNumber()
    {
        // Arrange
        var birthDate = new DateTime(1994, 12, 5);
        var sex = PatientSex.Male;

        // Act
        var nationalNumber = _nationalNumberGenerator.GenerateRandomNationalNumber(birthDate, sex);

        // Assert
        var year = int.Parse(nationalNumber.Substring(0, 2));
        var month = int.Parse(nationalNumber.Substring(2, 2));
        var day = int.Parse(nationalNumber.Substring(4, 2));
        var index = int.Parse(nationalNumber.Substring(6, 3));
        var modulo = int.Parse(nationalNumber.Substring(9, 2));
        var combined = long.Parse(nationalNumber.Substring(0, 9));

        Assert.Equal(94, year);
        Assert.Equal(12, month);
        Assert.Equal(5, day);
        Assert.Equal(1, (index % 2)); // Male should produce an odd index
        Assert.Equal((int)(97 - combined % 97), modulo);
    }

    [Fact]
    public void ShouldGenerateCorrectFemaleNationalNumber()
    {
        // Arrange
        var birthDate = new DateTime(1994, 12, 5);
        var sex = PatientSex.Female;

        // Act
        var nationalNumber = _nationalNumberGenerator.GenerateRandomNationalNumber(birthDate, sex);

        // Assert
        var year = int.Parse(nationalNumber.Substring(0, 2));
        var month = int.Parse(nationalNumber.Substring(2, 2));
        var day = int.Parse(nationalNumber.Substring(4, 2));
        var index = int.Parse(nationalNumber.Substring(6, 3));
        var modulo = int.Parse(nationalNumber.Substring(9, 2));
        var combined = long.Parse(nationalNumber.Substring(0, 9));

        Assert.Equal(94, year);
        Assert.Equal(12, month);
        Assert.Equal(5, day);
        Assert.Equal(0, (index % 2)); // Female should produce an even index
        Assert.Equal((int)(97 - combined % 97), modulo);
    }

    [Fact]
    public void ShouldGenerateCorrectNationalNumberAfter2000()
    {
        // Arrange
        var birthDate = new DateTime(2001, 12, 5);
        var sex = PatientSex.Male;

        // Act
        var nationalNumber = _nationalNumberGenerator.GenerateRandomNationalNumber(birthDate, sex);

        // Assert
        var year = int.Parse(nationalNumber.Substring(0, 2));
        var month = int.Parse(nationalNumber.Substring(2, 2));
        var day = int.Parse(nationalNumber.Substring(4, 2));
        var index = int.Parse(nationalNumber.Substring(6, 3));
        var modulo = int.Parse(nationalNumber.Substring(9, 2));
        var combined = long.Parse("2" + nationalNumber.Substring(0, 9));

        Assert.Equal(1, year);
        Assert.Equal(12, month);
        Assert.Equal(5, day);
        Assert.Equal(1, (index % 2)); // Male should produce an odd index
        Assert.Equal((int)(97 - combined % 97), modulo);
    }
}
