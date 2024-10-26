namespace MOTLookup.Models.ApiResponse
{
    public sealed class MOTTestApiResponse
    {
        public MOTTestApiResponse(string testResult, 
            int odometerValue, 
            string odometerUnit,
            string motTestNumber, 
            DateTime expiryDate,
            DateTime completedDate,
            List<RfrAndComment> rfrAndComments) 
        {
            TestResult = testResult;
            OdometerValue = odometerValue;
            OdometerUnit = odometerUnit;
            MotTestNumber = motTestNumber;
            ExpiryDate = expiryDate;
            CompletedDate = completedDate;
            RfrAndComments = rfrAndComments;
        }

        public string TestResult { get; private set; }

        public int OdometerValue { get; private set; }

        public string OdometerUnit { get; private set; }

        public string MotTestNumber { get; private set; }

        public DateTime ExpiryDate { get; private set; }

        public DateTime CompletedDate { get; private set; }

        public List<RfrAndComment> RfrAndComments { get; private set; }
    }

    public class RfrAndComment
    {
        public RfrAndComment(string text, string type)
        {
            Text = text;
            Type = type;
        }

        public string Text { get; private set; }

        public string Type { get; private set; }
    }
}
