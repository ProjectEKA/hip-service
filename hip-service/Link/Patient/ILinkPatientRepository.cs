using System;
using System.Threading.Tasks;
using hip_service.Link.Patient.Models;

namespace hip_service.Link.Patient
{
    public interface ILinkPatientRepository
    {
        Task<Tuple<LinkRequest, Exception>> SaveRequestWith(string linkReferenceNumber,
            string consentManagerId, string consentManagerUserId, string patientReferenceNumber,
            string[] careContextReferenceNumbers);

        Task<Tuple<LinkRequest, Exception>> GetPatientFor(string linkReferenceNumber);
    }
}