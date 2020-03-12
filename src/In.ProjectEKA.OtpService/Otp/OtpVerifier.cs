namespace In.ProjectEKA.OtpService.Otp
{
    using System.Threading.Tasks;
    using Common;

    public class OtpVerifier
    {
        private readonly IOtpRepository otpRepository;

        public OtpVerifier(IOtpRepository otpRepository)
        {
            this.otpRepository = otpRepository;
        }

        public async Task<Response> CheckOtpValue(string sessionId, string value)
        {
            var otpRequest = await otpRepository.GetWith(sessionId);
            return otpRequest.Map(o => o.OtpToken == value
                    ? new Response(ResponseType.OtpValid, "Valid OTP")
                    : new Response(ResponseType.OtpInvalid, "Invalid Otp"))
                .ValueOr(new Response(ResponseType.InternalServerError, "Session Id Not Found"));
        }
    }
}