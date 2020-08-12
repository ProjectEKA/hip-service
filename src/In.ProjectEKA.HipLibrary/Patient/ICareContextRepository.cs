using System.Collections.Generic;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipLibrary.Patient
{
    public interface ICareContextRepository
    {
        Task<IEnumerable<CareContextRepresentation>> GetCareContexts(string patientReferenceNumber);
    }
}
