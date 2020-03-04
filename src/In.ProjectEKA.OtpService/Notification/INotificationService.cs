namespace In.ProjectEKA.OtpService.Notification
{
    using System.Threading.Tasks;
    using Common;

    public interface INotificationService
    {
        public Task<Response> SendNotification(Notification notification);
    }
}