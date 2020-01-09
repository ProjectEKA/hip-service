using System;
using System.Threading.Tasks;
using hip_service.Link.Patient.Models;
using Optional;

namespace hip_service.Link.Patient
{
    public interface ILinkPatientRepository
    {
        
        public Task<Tuple<LinkRequest, Exception>> SaveLinkPatientDetails(string linkReferenceNumber,
            string consentManagerId, string consentManagerUserId, string patientReferenceNumber,
            string[] careContextReferenceNumbers);

        public Task<Tuple<LinkRequest, Exception>> GetPatientReferenceNumber(string linkReferenceNumber);
    }
}