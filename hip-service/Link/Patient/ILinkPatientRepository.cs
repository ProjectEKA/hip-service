using System;
using System.Threading.Tasks;
using hip_service.Link.Patient.Models;

namespace hip_service.Link.Patient
{
    using System.Collections.Generic;

    public interface ILinkPatientRepository
    {
        Task<Tuple<LinkRequest, Exception>> SaveRequestWith(
            string linkReferenceNumber,
            string consentManagerId,
            string consentManagerUserId,
            string patientReferenceNumber,
            IEnumerable<string> careContextReferenceNumbers);

        Task<Tuple<LinkRequest, Exception>> GetPatientFor(string linkReferenceNumber);
    }
}