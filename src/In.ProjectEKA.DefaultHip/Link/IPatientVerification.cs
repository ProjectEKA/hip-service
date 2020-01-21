using System.Threading.Tasks;

namespace In.ProjectEKA.DefaultHip.Link
{
    public interface IPatientVerification
    { 
        public Task<OtpMessage> SendTokenFor(Session session);
        public Task<OtpMessage> Verify(string sessionId, string value);
    }
}