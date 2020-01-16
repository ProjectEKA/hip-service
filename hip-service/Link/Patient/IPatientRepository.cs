using hip_service.Discovery.Patient.Model;
using Optional;

namespace hip_service.Link.Patient
{
    public interface IPatientRepository
    {
        Option<Discovery.Patient.Model.Patient> PatientWith(string referenceNumber);
        
        Option<CareContext> ProgramInfoWith(string patientReferenceNumber, string programReferenceNumber);
    }
}