using System.Threading.Tasks;

namespace otp_server.Otp
{
    public interface IOtpService
    {
        public Task<OtpResponse> GenerateOtp(OtpGenerationRequest otpGenerationRequest);
        public Task<OtpResponse> CheckOtpValue(string sessionId, string value);
    }
}