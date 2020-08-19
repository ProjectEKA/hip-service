namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using HipLibrary.Matcher;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Link;
    using Link.Model;
    using Microsoft.Extensions.Logging;

    public class PatientDiscovery
    {
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;
        private readonly ILinkPatientRepository linkPatientRepository;
        private readonly ILogger<PatientDiscovery> logger;
        private readonly IMatchingRepository matchingRepository;
        private readonly IPatientRepository patientRepository;

        public PatientDiscovery(
            IMatchingRepository matchingRepository,
            IDiscoveryRequestRepository discoveryRequestRepository,
            ILinkPatientRepository linkPatientRepository,
            IPatientRepository patientRepository,
            ILogger<PatientDiscovery> logger)
        {
            this.matchingRepository = matchingRepository;
            this.discoveryRequestRepository = discoveryRequestRepository;
            this.linkPatientRepository = linkPatientRepository;
            this.patientRepository = patientRepository;
            this.logger = logger;
        }

        public virtual async Task<ValueTuple<DiscoveryRepresentation, ErrorRepresentation>> PatientFor(
            DiscoveryRequest request)
        {
            if (await AlreadyExists(request.TransactionId))
            {
                logger.Log(LogLevel.Error, LogEvents.Discovery, "Discovery Request already exists");
                return (null,
                    new ErrorRepresentation(new Error(ErrorCode.DuplicateDiscoveryRequest,
                        "Discovery Request already exists")));
            }

            var (linkedAccounts, exception) = await linkPatientRepository.GetLinkedCareContexts(request.Patient.Id);

            if (exception != null)
            {
                logger.Log(LogLevel.Critical, LogEvents.Discovery, exception, "Failed to get care contexts");
                return (null,
                    new ErrorRepresentation(new Error(ErrorCode.FailedToGetLinkedCareContexts,
                        "Failed to get Linked Care Contexts")));
            }

            var linkedCareContexts = linkedAccounts.ToList();
            if (HasAny(linkedCareContexts))
            {
                logger.Log(LogLevel.Information,
                    LogEvents.Discovery,
                    "User has already linked care contexts: {TransactionID}",
                    request.TransactionId);
                return await patientRepository.PatientWith(linkedCareContexts.First().PatientReferenceNumber)
                    .Map(async patient =>
                    {
                        await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                            request.Patient.Id,
                            patient.Identifier));
                        return (new DiscoveryRepresentation(patient.ToPatientEnquiryRepresentation(
                            GetUnlinkedCareContexts(linkedCareContexts, patient))),
                            (ErrorRepresentation) null);
                    })
                    .ValueOr(Task.FromResult(((DiscoveryRepresentation) null,
                        new ErrorRepresentation(new Error(ErrorCode.NoPatientFound,
                            ErrorMessage.NoPatientFound)))));
            }

            var patients = await matchingRepository.Where(request);
            var (patientEnquiryRepresentation, error) =
                DiscoveryUseCase.DiscoverPatient(Filter.Do(patients, request).AsQueryable());
            if (patientEnquiryRepresentation == null)
            {
                logger.Log(LogLevel.Error,
                    LogEvents.Discovery,
                    "Error {@Error} for: {TransactionID}",
                    error,
                    request.TransactionId);
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