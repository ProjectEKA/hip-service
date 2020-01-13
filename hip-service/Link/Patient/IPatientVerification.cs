#nullable enable
using System.Threading.Tasks;
using hip_service.OTP;
using HipLibrary.Patient.Model.Response;

namespace hip_service.Link.Patient
{
    public interface IPatientVerification
    {
        Task<Error?> SendTokenFor(Session session);
        Task<Error?> Verify(string sessionId, string value);
    }
}