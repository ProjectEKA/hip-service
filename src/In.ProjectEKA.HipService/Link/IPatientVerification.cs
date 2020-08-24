namespace In.ProjectEKA.HipService.Link
{
    using System.Threading.Tasks;

    public interface IPatientVerification
    {
        public Task<OtpMessage> SendTokenFor(Session session);

        public Task<OtpMessage> Verify(string sessionId, string value);
    }
}