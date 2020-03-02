namespace In.ProjectEKA.OtpService.Notification
{
    using System.Threading.Tasks;

    public interface INotificationService
    {
        public Task<NotificationResponse> SendNotification(NotificationMessage notificationMessage);
    }
}