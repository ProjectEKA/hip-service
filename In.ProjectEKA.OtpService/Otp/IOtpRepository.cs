using System.Threading.Tasks;
using Optional;
using OtpServer.Otp.Model;

namespace OtpServer.Otp
{
    public interface IOtpRepository
    {
        Task<OtpResponse> Save(string otp, string sessionId);
        Task<Option<OtpRequest>> GetWith(string sessionId);
    }
}