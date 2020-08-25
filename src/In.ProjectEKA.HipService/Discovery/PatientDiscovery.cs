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
    using In.ProjectEKA.HipService.OpenMrs;
    using Link;
    using Microsoft.Extensions.Logging;
    using Logger;
    using Common;


    public class PatientDiscovery: IPatientDiscovery
    {
        private readonly IMatchingRepository matchingRepository;
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;
        private readonly ILinkPatientRepository linkPatientRepository;
        private readonly IPatientRepository patientRepository;
        private readonly ICareContextRepository careContextRepository;
        private readonly ILogger<PatientDiscovery> logger;

        public PatientDiscovery(
            IMatchingRepository matchingRepository,
            IDiscoveryRequestRepository discoveryRequestRepository,
            ILinkPatientRepository linkPatientRepository,
            IPatientRepository patientRepository,
            ICareContextRepository careContextRepository,
            ILogger<PatientDiscovery> logger)
        {
            this.matchingRepository = matchingRepository;
            this.discoveryRequestRepository = discoveryRequestRepository;
            this.linkPatientRepository = linkPatientRepository;
            this.patientRepository = patientRepository;
            this.careContextRepository = careContextRepository;
            this.logger = logger;
        }

        public virtual async Task<ValueTuple<DiscoveryRepresentation, ErrorRepresentation>> PatientFor(
            DiscoveryRequest request)
        {
            if (await AlreadyExists(request.TransactionId))
            {
                logger.Log(LogLevel.Error, LogEvents.Discovery, "Discovery Request already exists for {request.TransactionId}.");
                return (null,
                    new ErrorRepresentation(new Error(ErrorCode.DuplicateDiscoveryRequest, "Discovery Request already exists")));
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
                var patient = await patientRepository.PatientWithAsync(linkedCareContexts.First().PatientReferenceNumber);
                return await patient
                    .Map(async patient =>
                    {
                        await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                            request.Patient.Id, patient.Identifier));
                        return (new DiscoveryRepresentation(patient.ToPatientEnquiryRepresentation(
                                GetUnlinkedCareContexts(linkedCareContexts, patient))),
                            (ErrorRepresentation) null);
                    })
                    .ValueOr(Task.FromResult(GetError(ErrorCode.NoPatientFound, ErrorMessage.NoPatientFound)));
            }
            IQueryable<Patient> patients;

            try {
                patients = await matchingRepository.Where(request);
            }
            catch (OpenMrsConnectionException)
            {
                return GetError(ErrorCode.OpenMrsConnection, ErrorMessage.HipConnection);
            }

            try
            {
                foreach (var patient in patients)
                {
                    var careContexts = await careContextRepository.GetCareContexts(patient.Identifier);
                    patient.CareContexts = careContexts;
                }
            }
            catch (OpenMrsFormatException e)
            {
                logger.Log(LogLevel.Error,
                    LogEvents.Discovery,$"Could not get care contexts for transaction {request.TransactionId}.", e);
                return GetError(ErrorCode.CareContextConfiguration, ErrorMessage.HipConfiguration);
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

        private ValueTuple<DiscoveryRepresentation, ErrorRepresentation> GetError(ErrorCode errorCode, string errorMessage) {
            return (null, new ErrorRepresentation(new Error(errorCode, errorMessage)));
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
