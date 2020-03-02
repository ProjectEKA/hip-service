namespace In.ProjectEKA.OtpService.Notification
{
    public class NotificationContent
    {
        public string Requester { get; }
        public string ConsentRequestId { get; }
        public string HiTypes { get; }
        public string DeepLinkUrl { get; }

        public NotificationContent(string requester, string consentRequestId, string hiTypes, string deepLinkUrl)
        {
            Requester = requester;
            ConsentRequestId = consentRequestId;
            HiTypes = hiTypes;
            DeepLinkUrl = deepLinkUrl;
        }
    }
}