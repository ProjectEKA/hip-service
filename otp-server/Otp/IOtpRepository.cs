using System.Threading.Tasks;
using Optional;
using OtpServer.Otp.Model;

namespace OtpServer.Otp
{
    public interface IOtpRepository
    {
        public Task<OtpResponse> Save(string otp, string sessionId);
        public Task<Option<OtpRequest>> GetOtp(string sessionId);
    }
}