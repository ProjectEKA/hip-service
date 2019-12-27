using System.Linq;
using System.Threading.Tasks;

namespace hip_service.Discovery.Patient
{
    public interface IPatientRepository
    {
        Task<IQueryable<hip_library.Patient.models.Patient>> SearchPatients(string phoneNumber,
            string caseReferenceNumber, string firstName, string lastName);
    }    
}