using MOTLookup.Utilities.Validators;

namespace MOTLookup.Utilities.Tests.ValidatorsTests
{

    public class RegistrationValidatorTests
    {
        [Theory]
        [InlineData("AB12 CDE", true)]
        [InlineData("A123 BCD", true)]
        [InlineData("ABC 123D", true)]
        [InlineData("1234 ABC", true)]
        [InlineData("ABCD 123", true)]
        [InlineData("A1", true)] // Personalized plate
        [InlineData("AB12CDE", true)]
        [InlineData("INVALID", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("12345678", false)]
        public void IsValidUKRegistration_ReturnsExpectedResult(string registration, bool expected)
        {
            bool result = RegistrationValidator.IsValidUKRegistration(registration);
            Assert.Equal(expected, result);
        }
    }

}