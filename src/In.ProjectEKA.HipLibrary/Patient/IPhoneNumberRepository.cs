using System.Threading.Tasks;

namespace In.ProjectEKA.HipLibrary.Patient
{
    public interface IPhoneNumberRepository
    {
        Task<string> GetPhoneNumber(string patientReferenceNumber);
    }
}
