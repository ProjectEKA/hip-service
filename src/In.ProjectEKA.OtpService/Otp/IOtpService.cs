namespace In.ProjectEKA.OtpService.Otp
{
    using System.Threading.Tasks;

    public interface IOtpService
    { 
        Task<OtpResponse> GenerateOtp(OtpGenerationRequest otpGenerationRequest);
        Task<OtpResponse> CheckOtpValue(string sessionId, string value);
    }
}