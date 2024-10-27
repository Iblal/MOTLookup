using System.Text.RegularExpressions;

namespace MOTLookup.Utilities.Validators
{
    public static class RegistrationValidator
    {
        public static bool IsValidUKRegistration(string registration)
        {
            if (string.IsNullOrWhiteSpace(registration))
                return false;

            // Remove any spaces and convert to uppercase
            registration = registration.Replace(" ", "").ToUpperInvariant();

            // General plate formats
            string currentFormat = @"^[A-Z]{2}[0-9]{2}[A-Z]{3}$";
            string prefixFormat = @"^[A-Z][0-9]{1,3}[A-Z]{3}$";
            string suffixFormat = @"^[A-Z]{3}[0-9]{1,3}[A-Z]$";
            string datelessFormat1 = @"^[A-Z]{1,4}[0-9]{1,4}$";
            string datelessFormat2 = @"^[0-9]{1,4}[A-Z]{1,4}$";

            //Specific plate formates
            string personalizedFormat1 = @"^[0-9]{1,2}[A-Z]{1,3}$";
            string personalizedFormat2 = @"^[A-Z]{1,2}[0-9]{1,3}$";
            string personalizedFormat3 = @"^[A-Z]{1,3}[0-9]{1,2}$";
            string personalizedFormat4 = @"^[0-9]{1,3}[A-Z]{1,2}$";

            var regexes = new[]
            {
        new Regex(currentFormat),
        new Regex(prefixFormat),
        new Regex(suffixFormat),
        new Regex(datelessFormat1),
        new Regex(datelessFormat2),
        new Regex(personalizedFormat1),
        new Regex(personalizedFormat2),
        new Regex(personalizedFormat3),
        new Regex(personalizedFormat4)
    };

            foreach (var regex in regexes)
            {
                if (regex.IsMatch(registration))
                {
                    return true;
                }
            }

            return false;
        }

    }

}
