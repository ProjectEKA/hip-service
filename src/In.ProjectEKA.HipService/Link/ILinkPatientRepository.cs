namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model;

    public interface ILinkPatientRepository
    {
        Task<Tuple<LinkRequest, Exception>> SaveRequestWith(
            string linkReferenceNumber,
            string consentManagerId,
            string consentManagerUserId,
            string patientReferenceNumber,
            IEnumerable<string> careContextReferenceNumbers);

        Task<Tuple<LinkRequest, Exception>> GetPatientFor(string linkReferenceNumber);

        Task<Tuple<IEnumerable<LinkRequest>, Exception>> GetLinkedCareContexts(string consentManagerUserId);
    }
}