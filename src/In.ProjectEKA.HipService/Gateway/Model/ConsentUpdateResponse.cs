namespace In.ProjectEKA.HipService.Common.Model
{
    public class ConsentUpdateResponse
    {
        public string ConsentId { get; }
        public string Status { get; }
        public ConsentUpdateResponse(string status, string consentId)
        {
            ConsentId = consentId;
            Status = status;
        }
    }
}