namespace MOTLookup.Models.ApiResponse
{
    public sealed class MOTAPIResponse
    {
        public MOTAPIResponse(string registration, 
            string fuelType, 
            string make, 
            string model, 
            string primaryColour, 
            DateTime firstUsedDate,
            List<MOTAPIResponse> motTests) 
        {
            Registration = registration;
            FuelType = fuelType;
            Make = make;
            Model = model;
            PrimaryColour = primaryColour;
            FirstUsedDate = firstUsedDate;
            MotTests = motTests;
        }

        public string Registration { get; private set; }

        public string FuelType { get; private set; }

        public string Make { get; private set; }

        public string Model { get; private set; }

        public string PrimaryColour { get; private set; }

        public DateTime FirstUsedDate { get; private set; }

        public List<MOTAPIResponse> MotTests { get; private set; }

    }
}
