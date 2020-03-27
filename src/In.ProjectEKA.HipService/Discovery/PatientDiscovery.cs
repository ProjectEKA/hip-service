namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Link;
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
            if (await AlreadyExists(request.TransactionId))
            {
                return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(null,
                    new ErrorRepresentation(new Error(ErrorCode.DuplicateDiscoveryRequest, "Request already exists")));
            }

            var (linkRequests, exception) = await linkPatientRepository.GetLinkedCareContexts(request.Patient.Id);
            if (exception != null)
            {
                return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(null,
                    new ErrorRepresentation(new Error(ErrorCode.FailedToGetLinkedCareContexts,
                        "Failed to get Linked Care Contexts")));
            }

            if (HasAny(linkRequests))
            {
                return await patientRepository.PatientWith(linkRequests.First().PatientReferenceNumber)
                    .Map(async patient =>
                    {
                        await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                            request.Patient.Id));
                        return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(
                            new DiscoveryRepresentation(patient.ToPatientEnquiryRepresentation(
                                GetUnlinkedCareContexts(linkRequests, patient))),
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

        private async Task<bool> AlreadyExists(string transactionId)
        {
            return await discoveryRequestRepository.RequestExistsFor(transactionId);
        }

        private static bool HasAny(IEnumerable<LinkRequest> linkRequests)
        {
            return linkRequests.Any(linkRequest => true);
        }

        private static IEnumerable<CareContextRepresentation> GetUnlinkedCareContexts(
            IEnumerable<LinkRequest> linkRequests,
            Patient patient)
        {
            var allLinkedCareContexts = linkRequests
                .SelectMany(linkRequest => linkRequest.CareContexts)
                .ToList();
            return patient.CareContexts
                .Where(careContext =>
                    allLinkedCareContexts.Find(linkedCareContext =>
                        linkedCareContext.CareContextNumber == careContext.ReferenceNumber) == null);
        }
    }
}