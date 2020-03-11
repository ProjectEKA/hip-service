namespace In.ProjectEKA.OtpService.Otp
{
    using System.Threading.Tasks;
    using Common;

    public interface IOtpSender
    {
        Task<Response> GenerateOtp(OtpGenerationRequest otpGenerationRequest);
    }
}