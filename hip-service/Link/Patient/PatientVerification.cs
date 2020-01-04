#nullable enable
using System.Threading.Tasks;
using hip_library.Patient.models;
using hip_service.OTP;

namespace hip_service.Link.Patient
{
    public class PatientVerification: IPatientVerification
    {
        private readonly OtpVerification _otpVerification;

        public PatientVerification(OtpVerification otpVerification)
        {
            _otpVerification = otpVerification;
        }

        public async Task<Error?> GenerateVerificationToken(Session session)
        {
            return await _otpVerification.GenerateOtp(session);
        }
        
        public async Task<Error?> AuthenticateVerificationToken(string sessionId, string value)
        {
            return await _otpVerification.CheckOtpValue(sessionId, value);
        }
    }
}