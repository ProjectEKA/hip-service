#nullable enable
using System.Threading.Tasks;
using hip_service.OTP;
using HipLibrary.Patient.Models.Response;

namespace hip_service.Link.Patient
{
    public class PatientVerification: IPatientVerification
    {
        private readonly OtpVerification otpVerification;

        public PatientVerification(OtpVerification otpVerification)
        {
            this.otpVerification = otpVerification;
        }

        public async Task<Error?> SendTokenFor(Session session)
        {
            return await otpVerification.GenerateOtp(session);
        }
        
        public async Task<Error?> Verify(string sessionId, string value)
        {
            return await otpVerification.CheckOtpValue(sessionId, value);
        }
    }
}