namespace In.ProjectEKA.OtpService.Clients
{
    using System.Threading.Tasks;
    using Common;
    using Notification;

    public interface ISmsClient
    {
        public Task<Response> Send(string phoneNumber, string message);
    }
}