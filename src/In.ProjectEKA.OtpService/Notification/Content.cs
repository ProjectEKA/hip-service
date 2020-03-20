namespace In.ProjectEKA.OtpService.Notification
{
    public class Content
    {
        public string Requester { get; }
        public string ConsentRequestId { get; }
        public string HiTypes { get; }
        public string DeepLinkUrl { get; }

        public Content(string requester, string consentRequestId, string hiTypes, string deepLinkUrl)
        {
            Requester = requester;
            ConsentRequestId = consentRequestId;
            HiTypes = hiTypes;
            DeepLinkUrl = deepLinkUrl;
        }
    }
}