namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model;

    public interface ILinkPatientRepository
    {
        Task<ValueTuple<LinkRequest, Exception>> SaveRequestWith(
            string linkReferenceNumber,
            string consentManagerId,
            string consentManagerUserId,
            string patientReferenceNumber,
            IEnumerable<string> careContextReferenceNumbers);

        Task<ValueTuple<LinkRequest, Exception>> GetPatientFor(string linkReferenceNumber);

        Task<ValueTuple<IEnumerable<LinkRequest>, Exception>> GetLinkedCareContexts(string consentManagerUserId);
    }
}