using System.Collections.Generic;
using System.Linq;
using hip_library.Patient;
using hip_library.Patient.models.domain;
using hip_library.Patient.models.dto;

namespace hip_service.Discovery.Patients
{
    public class PatientDiscovery : IDiscovery
    {
        private readonly IPatientRepository patientRepository;

        public PatientDiscovery(IPatientRepository patientRepository)
        {
            this.patientRepository = patientRepository;
        }
        
        public List<Patient> GetPatients(PatientRequest request)
        {
            return patientRepository.GetPatients(request.PhoneNumber, request.CaseId, request.FirstName,
                request.LastName).ToList();
        }
    }
}