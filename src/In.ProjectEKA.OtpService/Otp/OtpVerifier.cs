namespace In.ProjectEKA.OtpService.Otp
{
    using System;
    using System.Threading.Tasks;
    using Common;

    public class OtpVerifier
    {
        private readonly IOtpRepository otpRepository;
        private readonly OtpProperties otpProperties;

        public OtpVerifier(IOtpRepository otpRepository, OtpProperties otpProperties)
        {
            this.otpRepository = otpRepository;
            this.otpProperties = otpProperties;
        }

        public async Task<Response> VerifyFor(string sessionId, string otp)
        {
            bool IsExpired(DateTime requestedAt)
            {
                return requestedAt.AddMinutes(otpProperties.ExpiryInMinutes) < DateTime.Now.ToUniversalTime();
            }

            bool IsSame(string actualToken)
            {
                return actualToken == otp;
            }

            var otpRequest = await otpRepository.GetWith(sessionId);
            return otpRequest
                .Map(o => !IsSame(o.OtpToken)
                    ? new Response(ResponseType.OtpInvalid, "Invalid Otp")
                    : IsExpired(o.RequestedAt)
                        ? new Response(ResponseType.OtpExpired, "Otp expired")
                        : new Response(ResponseType.OtpValid, "Valid OTP"))
                .ValueOr(new Response(ResponseType.InternalServerError, "Session Id Not Found"));
        }
    }
}