namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HipLibrary.Matcher;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using In.ProjectEKA.HipService.Link.Model;
    using Link;
    using Logger;

    public class PatientDiscovery: IPatientDiscovery
    {
        private readonly IMatchingRepository matchingRepository;
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;
        private readonly ILinkPatientRepository linkPatientRepository;
        private readonly IPatientRepository patientRepository;
        private readonly ICareContextRepository careContextRepository;

        public PatientDiscovery(
            IMatchingRepository matchingRepository,
            IDiscoveryRequestRepository discoveryRequestRepository,
            ILinkPatientRepository linkPatientRepository,
            IPatientRepository patientRepository,
            ICareContextRepository careContextRepository)
        {
            this.matchingRepository = matchingRepository;
            this.discoveryRequestRepository = discoveryRequestRepository;
            this.linkPatientRepository = linkPatientRepository;
            this.patientRepository = patientRepository;
            this.careContextRepository = careContextRepository;
        }

        public virtual async Task<ValueTuple<DiscoveryRepresentation, ErrorRepresentation>> PatientFor(
            DiscoveryRequest request)
        {
            if (await AlreadyExists(request.TransactionId))
            {
                Log.Information($"Discovery Request already exists for {request.TransactionId}.");
                return (null,
                    new ErrorRepresentation(new Error(ErrorCode.DuplicateDiscoveryRequest, "Discovery Request already exists")));
            }

            var (linkedAccounts, exception) = await linkPatientRepository.GetLinkedCareContexts(request.Patient.Id);

            if (exception != null)
            {
                Log.Error(exception);
                return (null,
                    new ErrorRepresentation(new Error(ErrorCode.FailedToGetLinkedCareContexts,
                        "Failed to get Linked Care Contexts")));
            }

            var linkedCareContexts = linkedAccounts.ToList();
            if (HasAny(linkedCareContexts))
            {
                Log.Information($"Found already linked care contexts for transaction {request.TransactionId}.");

                return await patientRepository.PatientWith(linkedCareContexts.First().PatientReferenceNumber)
                    .Map(async patient =>
                    {
                        await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                            request.Patient.Id, patient.Identifier));
                        return (new DiscoveryRepresentation(patient.ToPatientEnquiryRepresentation(
                                GetUnlinkedCareContexts(linkedCareContexts, patient))),
                            (ErrorRepresentation) null);
                    })
                    .ValueOr(Task.FromResult(((DiscoveryRepresentation) null,
                        new ErrorRepresentation(new Error(ErrorCode.NoPatientFound,
                            ErrorMessage.NoPatientFound)))));
            }

            var patients = await matchingRepository.Where(request);
            foreach (var patient in patients)
            {
                var careContexts = await careContextRepository.GetCareContexts(patient.Identifier);
                patient.CareContexts = careContexts;
            }

            var (patientEnquiryRepresentation, error) =
                DiscoveryUseCase.DiscoverPatient(Filter.Do(patients, request).AsQueryable());
            if (patientEnquiryRepresentation == null)
            {
                Log.Information($"No matching unique patient found for transaction {request.TransactionId}.", error);
                return (null, error);
            }

            await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                request.Patient.Id, patientEnquiryRepresentation.ReferenceNumber));
            return (new DiscoveryRepresentation(patientEnquiryRepresentation), null);
        }

        private async Task<bool> AlreadyExists(string transactionId)
        {
            return await discoveryRequestRepository.RequestExistsFor(transactionId);
        }

        private static bool HasAny(IEnumerable<LinkedAccounts> linkedAccounts)
        {
            return linkedAccounts.Any(account => true);
        }

        private static IEnumerable<CareContextRepresentation> GetUnlinkedCareContexts(
            IEnumerable<LinkedAccounts> linkedAccounts,
            Patient patient)
        {
            var allLinkedCareContexts = linkedAccounts
                .SelectMany(account => account.CareContexts)
                .ToList();

            return patient.CareContexts
                .Where(careContext =>
                    allLinkedCareContexts.Find(linkedCareContext =>
                        linkedCareContext == careContext.ReferenceNumber) == null);
        }
    }
}