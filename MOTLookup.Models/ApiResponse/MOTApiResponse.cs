namespace MOTLookup.Models.ApiResponse
{
    public sealed class MOTAPIResponse
    {
        public string Registration { get; set; }

        public string FuelType { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public string PrimaryColour { get; set; }

        public DateTime FirstUsedDate { get; set; }

        public List<MOTAPIResponse> MotTests { get; set; }

    }
}
