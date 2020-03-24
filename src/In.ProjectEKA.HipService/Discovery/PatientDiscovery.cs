namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using In.ProjectEKA.HipService.Link;
    using In.ProjectEKA.HipService.Link.Model;

    public class PatientDiscovery : IDiscovery
    {
        private readonly Filter filter;
        private readonly IMatchingRepository matchingRepository;
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;
        private readonly ILinkPatientRepository linkPatientRepository;
        private readonly IPatientRepository patientRepository;

        public PatientDiscovery(
            IMatchingRepository matchingRepository,
            IDiscoveryRequestRepository discoveryRequestRepository,
            ILinkPatientRepository linkPatientRepository,
            IPatientRepository patientRepository)
        {
            this.matchingRepository = matchingRepository;
            this.discoveryRequestRepository = discoveryRequestRepository;
            this.linkPatientRepository = linkPatientRepository;
            this.patientRepository = patientRepository;
            filter = new Filter();
        }

        public async Task<Tuple<DiscoveryRepresentation, ErrorRepresentation>> PatientFor(DiscoveryRequest request)
        {
            var (linkedAccounts, exception) = await linkPatientRepository.GetLinkedCareContexts(request.Patient.Id);
            if (exception != null)
            {
                return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(null,
                    new ErrorRepresentation(new Error(ErrorCode.FailedToGetLinkedCareContexts,
                        "Failed to get Linked Care Contexts")));
            }

            var linkedCareContexts = linkedAccounts.ToList();
            if (HasAny(linkedCareContexts))
            {
                return await patientRepository.PatientWith(linkedCareContexts.First().PatientReferenceNumber)
                    .Map(async patient =>
                    {
                        await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                            request.Patient.Id));
                        return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(
                            new DiscoveryRepresentation(patient.ToPatientEnquiryRepresentation(
                                GetUnlinkedCareContexts(linkedCareContexts, patient))),
                            null);
                    }).ValueOr(
                        Task.FromResult(new Tuple<DiscoveryRepresentation, ErrorRepresentation>(
                            null,
                            new ErrorRepresentation(new Error(ErrorCode.NoPatientFound,
                                ErrorMessage.NoPatientFound))))
                    );
            }

            var patients = await matchingRepository.Where(request);
            var (patientEnquiryRepresentation, error) = DiscoveryUseCase.DiscoverPatient(
                filter.Do(patients, request).AsQueryable());
            if (patientEnquiryRepresentation == null)
            {
                return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(null, error);
            }

            await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                request.Patient.Id));
            return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(
                new DiscoveryRepresentation(patientEnquiryRepresentation), null);
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