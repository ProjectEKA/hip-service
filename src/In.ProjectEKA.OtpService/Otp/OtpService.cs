namespace In.ProjectEKA.OtpService.Otp
{
    using System.Threading.Tasks;
    using Clients;
    using Common;

    public class OtpService: IOtpService
    {
        private readonly IOtpRepository otpRepository;
        private readonly IOtpGenerator otpGenerator;
        private readonly ISmsClient smsClient;
        
        public OtpService(IOtpRepository otpRepository, IOtpGenerator otpGenerator, ISmsClient smsClient)
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
                return await otpRepository.Save(otpValue,otpGenerationRequest.SessionId);
            }
            return sendOtp;
        }
        
        public async Task<Response> CheckOtpValue(string sessionId, string value)
        {
            var otpRequest = await otpRepository.GetWith(sessionId) ;
            return otpRequest.Map(o => o.OtpToken == value
                ? new Response(ResponseType.OtpValid,"Valid OTP")
                : new Response(ResponseType.OtpInvalid,"Invalid Otp"))
                .ValueOr(new Response(ResponseType.InternalServerError,"Session Id Not Found"));
        }
    }
}