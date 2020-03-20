namespace In.ProjectEKA.OtpService.Otp
{
    using System.Threading.Tasks;
    using Common;

    public class FakeOtpSender : IOtpSender
    {
        private readonly IOtpRepository otpRepository;

        public FakeOtpSender(IOtpRepository otpRepository)
        {
            this.otpRepository = otpRepository;
        }

        public async Task<Response> GenerateOtp(OtpGenerationRequest otpGenerationRequest)
        {
            return await otpRepository.Save("666666", otpGenerationRequest.SessionId);
        }
    }
}