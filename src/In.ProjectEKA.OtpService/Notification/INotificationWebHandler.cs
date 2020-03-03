namespace In.ProjectEKA.OtpService.Notification
{
    using System.Threading.Tasks;

    public interface INotificationWebHandler
    {
        public Task<NotificationResponse> Send(string phoneNumber, string message);
    }
}