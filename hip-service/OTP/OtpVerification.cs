using System;
using System.Threading.Tasks;
using HipLibrary.Patient.Models.Response;
namespace hip_service.OTP
{
    public class OtpVerification
    {
        private readonly IOtpRepository otpRepository;

        public OtpVerification(IOtpRepository otpRepository)
        {
            this.otpRepository = otpRepository;
        }

        public async Task<Error> GenerateOtp(Session session)
        {
            var (_, exception) = await otpRepository.Save("1234",session.SessionId);
            return exception != null ? new Error(ErrorCode.OtpInValid,"OTP not generated") : null;
        }

        public async Task<Error> CheckOtpValue(string sessionId, string value)
        {
            var (otpValue, exception) = await otpRepository.GetOtp(sessionId);
            return otpValue.OtpToken == value ? null : new Error(ErrorCode.OtpInValid, "OTP not valid");
        }
    }
}