#nullable enable
namespace In.ProjectEKA.HipService.Link.Patient
{
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model.Response;
    using OTP;

    public class PatientVerification : IPatientVerification
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