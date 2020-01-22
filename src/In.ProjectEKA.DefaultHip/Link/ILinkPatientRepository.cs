using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using In.ProjectEKA.DefaultHip.Link.Model;

namespace In.ProjectEKA.DefaultHip.Link
{
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