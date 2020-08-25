namespace In.ProjectEKA.HipService.Common.Model
{
    public class ConsentUpdateResponse
    {
        public ConsentUpdateResponse(string status, string consentId)
        {
            ConsentId = consentId;
            Status = status;
        }

        public string ConsentId { get; }
        public string Status { get; }
    }
}