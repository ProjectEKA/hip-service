namespace In.ProjectEKA.OtpService.Notification
{
    using Newtonsoft.Json.Linq;

    public class NotificationMessage
    {
        public string Id { get; }
        public Communication Communication { get; }
        public JObject NotificationContent { get; }
        public NotificationAction NotificationAction { get; }

        public NotificationMessage(string id,
                                   Communication communication,
                                   JObject notificationContent,
                                   NotificationAction notificationAction)
        {
            Id = id;
            Communication = communication;
            NotificationContent = notificationContent;
            NotificationAction = notificationAction;
        }
    }
}