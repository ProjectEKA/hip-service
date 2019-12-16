using System.Collections.Generic;
using hip_library.Patient.models.domain;

namespace hip_service.Discovery.Patients
{
    public interface IPatientRepository
    {
        IEnumerable<Patient> GetPatients(string phoneNumber, string caseId, string firstName, string lastName);
    }
}