using System;
using System.Threading.Tasks;
using hip_library.Patient.models;

namespace hip_service.OTP
{
    public class OtpGeneration
    {
        private readonly IOtpRepository _otpRepository;

        public OtpGeneration(IOtpRepository otpRepository)
        {
            _otpRepository = otpRepository;
        }

        public async Task<Error> GenerateOtp(string linkReferenceNumber)
        {
            var staticOtp = "1234";
            var (_, exception) = await _otpRepository.SaveOtpRequest(staticOtp,linkReferenceNumber);
            return exception != null ? new Error(ErrorCode.OtpInValid,"OTP not generated") : null;
        }

        public async Task<Error> CheckOtpValue(string linkReferenceNumber, string value)
        {
            var (otpValue, exception) = await _otpRepository.GetOtp(linkReferenceNumber);
            return otpValue == value ? null : new Error(ErrorCode.OtpInValid, "OTP not valid");
        }
    }
}