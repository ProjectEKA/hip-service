using System.Collections.Generic;
using hip_service.Discovery.Patient.Model;
using Optional;

namespace hip_service.Link.Patient
{
    public interface IPatientRepository
    {
        IEnumerable<Discovery.Patient.Model.Patient> GetAllPatientFromJson();
        public Option<Discovery.Patient.Model.Patient> GetPatientInfoWithReferenceNumber(string referenceNumber);
        public Option<CareContext> GetProgramInfo(string patientReferenceNumber, string programReferenceNumber);
    }
}