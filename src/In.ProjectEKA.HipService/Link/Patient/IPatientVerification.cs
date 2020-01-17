using hip_service.Link.Patient;

namespace In.ProjectEKA.HipService.Link.Patient
{
    using System.Threading.Tasks;
    public interface IPatientVerification
    { 
        public Task<OtpMessage> SendTokenFor(Session session);
        public Task<OtpMessage> Verify(string sessionId, string value);
    }
}