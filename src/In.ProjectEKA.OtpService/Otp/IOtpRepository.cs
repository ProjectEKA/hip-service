using System.Threading.Tasks;
using In.ProjectEKA.OtpService.Otp.Model;
using Optional;

namespace In.ProjectEKA.OtpService.Otp
{
    public interface IOtpRepository
    {
        Task<OtpResponse> Save(string otp, string sessionId);
        Task<Option<OtpRequest>> GetWith(string sessionId);
    }
}