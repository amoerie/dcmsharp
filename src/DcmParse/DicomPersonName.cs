namespace DcmParse;

public readonly record struct DicomPersonName(
    string? FamilyName,
    string? GivenName,
    string? MiddleName,
    string? NamePrefix,
    string? NameSuffix,
    string? IdeographicFamilyName,
    string? IdeographicGivenName,
    string? PhoneticFamilyName,
    string? PhoneticGivenName)
{

}
