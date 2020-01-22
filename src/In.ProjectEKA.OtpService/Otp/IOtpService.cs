using System.Threading.Tasks;

namespace In.ProjectEKA.OtpService.Otp
{
    public interface IOtpService
    { 
        Task<OtpResponse> GenerateOtp(OtpGenerationRequest otpGenerationRequest);
        Task<OtpResponse> CheckOtpValue(string sessionId, string value);
    }
}