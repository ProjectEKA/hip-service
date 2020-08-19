namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model;
    using Optional;

    public interface ILinkPatientRepository
    {
        Task<Tuple<LinkEnquires, Exception>> SaveRequestWith(
            string linkReferenceNumber,
            string consentManagerId,
            string consentManagerUserId,
            string patientReferenceNumber,
            IEnumerable<string> careContextReferenceNumbers);

        Task<Tuple<LinkEnquires, Exception>> GetPatientFor(string linkReferenceNumber);

        Task<Option<LinkedAccounts>> Save(
            string consentManagerUserId,
            string patientReferenceNumber,
            string linkReferenceNumber,
            IEnumerable<string> careContextReferenceNumbers);

        Task<Tuple<IEnumerable<LinkedAccounts>, Exception>> GetLinkedCareContexts(string consentManagerUserId);

        Task<Option<InitiatedLinkRequest>> Save(string requestId, string transactionId, string linkReferenceNumber);

        Task<Option<IEnumerable<InitiatedLinkRequest>>> Get(string linkReferenceNumber);

        Task<bool> Update(InitiatedLinkRequest linkRequest);
    }
}