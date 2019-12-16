using System.Collections.Generic;
using hip_library.Patient.models.domain;

namespace hip_service.Discovery.Patients
{
    public class PatientsResponse
    {
        public IEnumerable<Patient> Patients { get; }

        public PatientsResponse(IEnumerable<Patient> patients)
        {
            Patients = patients;
        }
    }
}