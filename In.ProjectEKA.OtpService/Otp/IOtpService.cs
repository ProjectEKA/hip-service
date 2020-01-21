using System.Threading.Tasks;

namespace OtpServer.Otp
{
    public interface IOtpService
    {
        Task<OtpResponse> GenerateOtp(OtpGenerationRequest otpGenerationRequest);
        Task<OtpResponse> CheckOtpValue(string sessionId, string value);
    }
}