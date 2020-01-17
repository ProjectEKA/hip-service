#nullable enable
namespace In.ProjectEKA.HipService.Link.Patient
{
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model.Response;

    public interface IPatientVerification
    {
        Task<Error?> SendTokenFor(Session session);

        Task<Error?> Verify(string sessionId, string value);
    }
}