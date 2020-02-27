namespace In.ProjectEKA.OtpService.Otp
{
    using System.Threading.Tasks;

    public class OtpService: IOtpService
    {
        private readonly IOtpRepository otpRepository;
        private readonly IOtpGenerator otpGenerator;
        private readonly IOtpWebHandler otpWebHandler;
        
        public OtpService(IOtpRepository otpRepository, IOtpGenerator otpGenerator, IOtpWebHandler otpWebHandler)
        {
            this.otpRepository = otpRepository;
            this.otpGenerator = otpGenerator;
            this.otpWebHandler = otpWebHandler;
        }

        public async Task<OtpResponse> GenerateOtp(OtpGenerationRequest otpGenerationRequest)
        {
            var otpValue = otpGenerator.GenerateOtp();
            var sendOtp = otpWebHandler.SendOtp(otpGenerationRequest.Communication.Value, otpValue);
            if (sendOtp.ResponseType == ResponseType.Success)
            {
                return await otpRepository.Save(otpValue,otpGenerationRequest.SessionId);
            }
            return sendOtp;
        }
        
        public async Task<OtpResponse> CheckOtpValue(string sessionId, string value)
        {
            var otpRequest = await otpRepository.GetWith(sessionId) ;
            return otpRequest.Map(o => o.OtpToken == value
                ? new OtpResponse(ResponseType.OtpValid,"Valid OTP")
                : new OtpResponse(ResponseType.OtpInvalid,"Invalid Otp"))
                .ValueOr(new OtpResponse(ResponseType.InternalServerError,"Session Id Not Found"));
        }
    }
}