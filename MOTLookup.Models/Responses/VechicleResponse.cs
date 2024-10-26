namespace MOTLookup.Models.Responses
{
    public sealed class VehicleResponse
    {
        public VehicleResponse(
            string make,
            string model, 
            string colour, 
            DateTime expiryDate, 
            int mileageAtLastMot)
        {
            Make = make;
            Model = model;
            Colour = colour;
            MotExpiryDate = expiryDate;
            MileageAtLastMot = mileageAtLastMot;
        }

        public string Make { get; private set; }

        public string Model { get; private set; }

        public string Colour { get; private set; }

        public int MileageAtLastMot { get; private set; }

        public DateTime MotExpiryDate { get; private set; }
    }
}
