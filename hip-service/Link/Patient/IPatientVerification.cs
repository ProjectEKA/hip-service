#nullable enable
using System.Threading.Tasks;
using hip_library.Patient.models;
using hip_service.OTP;

namespace hip_service.Link.Patient
{
    public interface IPatientVerification
    {
        public Task<Error?> GenerateVerificationToken(Session session);
        public Task<Error?> AuthenticateVerificationToken(string sessionId, string value);
    }
}