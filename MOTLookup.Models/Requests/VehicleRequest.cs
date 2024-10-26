namespace MOTLookup.Models.Requests
{
    public sealed class VehicleRequest
    {
        public VehicleRequest(string registrationNumber)
        {
            RegistrationNumber = registrationNumber;
        }

        public string RegistrationNumber { get; private set; }
    }
}
