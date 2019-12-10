using System.Collections.Generic;
using hip_library.Patient.models.domain;

namespace hip_service.Models.dto
{
    public class PatientsResponse
    {
        public List<Patient> Patients { get; }

        public PatientsResponse(List<Patient> patients)
        {
            Patients = patients;
        }
    }
}