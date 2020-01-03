using System;
using System.Linq;
using System.Threading.Tasks;
using hip_library.Patient.models;
using hip_library.Patient.models.dto;
using hip_service.Discovery.Patient.models;
using hip_service.Link.Patient.Models;

namespace hip_service.Link.Patient
{
    public interface ILinkPatientRepository
    {
        
        public  Task<Tuple<LinkRequest, Exception>> SaveLinkPatientDetails(string linkReferenceNumber, 
            string consentManagerId, string consentManagerUserId, string patientReferenceNumber,
            string[] careContextReferenceNumbers);

        public Task<Tuple<LinkRequest, Exception>> GetPatientReferenceNumber(string linkReferenceNumber);
    }
}