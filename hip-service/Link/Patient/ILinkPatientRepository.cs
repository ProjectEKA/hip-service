using System;
using System.Linq;
using System.Threading.Tasks;
using hip_library.Patient.models;
using hip_library.Patient.models.dto;

namespace hip_service.Link.Patient
{
    public interface ILinkPatientRepository
    {
        
        public Task<Tuple<PatientLinkReferenceResponse, Error>> LinkPatient(string patientReferenceNumber,
            string[] careContextReferenceNumbers);
    }
}