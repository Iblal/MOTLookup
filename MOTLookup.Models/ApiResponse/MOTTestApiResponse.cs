namespace MOTLookup.Models.ApiResponse
{
    public sealed class MOTTestApiResponse
    {

        public string TestResult { get; set; }

        public int OdometerValue { get; set; }

        public string OdometerUnit { get; set; }

        public string MotTestNumber { get; set; }

        public DateTime ExpiryDate { get; set; }

        public DateTime CompletedDate { get; set; }

        public List<RfrAndComment> RfrAndComments { get; set; }
    }

    public class RfrAndComment
    {
        public RfrAndComment(string text, string type)
        {
            Text = text;
            Type = type;
        }

        public string Text { get; set; }

        public string Type { get; set; }
    }
}
