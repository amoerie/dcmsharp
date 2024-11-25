using System.Text;

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
    public override string ToString()
    {
        // Calculate the initial capacity for the StringBuilder
        int capacity = 0;
        if (NamePrefix is not null)
        {
            capacity += NamePrefix.Length + 1; // +1 for space
        }

        if (GivenName is not null)
        {
            capacity += GivenName.Length + 1; // +1 for space
        }

        if (MiddleName is not null)
        {
            capacity += MiddleName.Length + 1; // +1 for space
        }

        if (FamilyName is not null)
        {
            capacity += FamilyName.Length;
        }

        if (NameSuffix is not null)
        {
            capacity += NameSuffix.Length + 2; // +2 for ", "
        }

        // Create the StringBuilder with the precomputed capacity
        var sb = new StringBuilder(capacity);

        // Append the prefix if present
        if (NamePrefix is not null)
        {
            sb.Append(NamePrefix).Append(' ');
        }

        // Append the given name if present
        if (GivenName is not null)
        {
            sb.Append(GivenName).Append(' ');
        }

        // Append the middle name if present
        if (MiddleName is not null)
        {
            sb.Append(MiddleName).Append(' ');
        }

        // Append the family name if present
        if (FamilyName is not null)
        {
            sb.Append(FamilyName);
        }

        // Append the suffix if present, ensuring proper formatting
        if (NameSuffix is not null)
        {
            if (sb.Length > 0) // Ensure there is a name before appending ", "
            {
                sb.Append(", ");
            }

            sb.Append(NameSuffix);
        }

        return sb.ToString();
    }
}
