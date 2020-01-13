using System.Threading.Tasks;
using Optional;
using otp_server.Otp.Models;

namespace otp_server.Otp
{
    public interface IOtpRepository
    {
        public Task<OtpResponse> Save(string otp, string sessionId);
        public Task<Option<OtpRequest>> GetOtp(string sessionId);
    }
}