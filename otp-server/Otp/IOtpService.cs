using System.Threading.Tasks;

namespace OtpServer.Otp
{
    public interface IOtpService
    {
        public Task<OtpResponse> GenerateOtp(OtpGenerationRequest otpGenerationRequest);
        public Task<OtpResponse> CheckOtpValue(string sessionId, string value);
    }
}