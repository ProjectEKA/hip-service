using System.Threading.Tasks;
using hip_service.OTP;
namespace hip_service.Link.Patient
{
    public interface IPatientVerification
    { 
        public Task<OtpMessage> SendTokenFor(Session session);
        public Task<OtpMessage> Verify(string sessionId, string value);
    }
}