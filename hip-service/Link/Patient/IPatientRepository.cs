using System.Collections.Generic;
using hip_service.Discovery.Patient.Model;
using Optional;

namespace hip_service.Link.Patient
{
    public interface IPatientRepository
    {
        IEnumerable<Discovery.Patient.Model.Patient> All();
        Option<Discovery.Patient.Model.Patient> PatientWith(string referenceNumber);
        Option<CareContext> ProgramInfoWith(string patientReferenceNumber, string programReferenceNumber);
    }
}