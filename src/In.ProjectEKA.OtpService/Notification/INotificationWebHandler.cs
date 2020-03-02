namespace In.ProjectEKA.OtpService.Notification
{
    public interface INotificationWebHandler
    {
        public NotificationResponse Send(string phoneNumber, string message);
    }
}