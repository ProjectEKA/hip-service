namespace In.ProjectEKA.OtpService.Otp
{
    using System.Threading.Tasks;
    using Common;

    public interface IOtpService
    { 
        Task<Response> GenerateOtp(OtpGenerationRequest otpGenerationRequest);
        Task<Response> CheckOtpValue(string sessionId, string value);
    }
}