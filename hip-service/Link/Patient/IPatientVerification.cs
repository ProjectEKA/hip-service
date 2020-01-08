using System.Threading.Tasks;
using hip_service.OTP;
using HipLibrary.Patient.Models.Response;

namespace hip_service.Link.Patient
{
    public interface IPatientVerification
    {
        public Task<Error?> SendTokenFor(Session session);
        public Task<Error?> Verify(string sessionId, string value);
    }
}