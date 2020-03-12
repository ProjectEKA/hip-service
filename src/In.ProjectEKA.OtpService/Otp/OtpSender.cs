namespace In.ProjectEKA.OtpService.Otp
{
    using System.Threading.Tasks;
    using Clients;
    using Common;

    public class OtpSender : IOtpSender
    {
        private readonly IOtpRepository otpRepository;
        private readonly IOtpGenerator otpGenerator;
        private readonly ISmsClient smsClient;

        public OtpSender(IOtpRepository otpRepository, IOtpGenerator otpGenerator, ISmsClient smsClient)
        {
            this.otpRepository = otpRepository;
            this.otpGenerator = otpGenerator;
            this.smsClient = smsClient;
        }

        public async Task<Response> GenerateOtp(OtpGenerationRequest otpGenerationRequest)
        {
            var otpValue = otpGenerator.GenerateOtp();
            var sendOtp = await smsClient.Send(otpGenerationRequest.Communication.Value, otpValue);
            if (sendOtp.ResponseType == ResponseType.Success)
            {
                return await otpRepository.Save(otpValue, otpGenerationRequest.SessionId);
            }

            return sendOtp;
        }
    }
}