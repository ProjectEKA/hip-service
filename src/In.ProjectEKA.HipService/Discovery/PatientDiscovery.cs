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

        public PatientDiscovery(
            IMatchingRepository patientRepository,
            IDiscoveryRequestRepository discoveryRequestRepository,
            ILinkPatientRepository linkPatientRepository)
        {
            matchingRepository = patientRepository;
            this.discoveryRequestRepository = discoveryRequestRepository;
            this.linkPatientRepository = linkPatientRepository;
            filter = new Filter();
        }

        public async Task<Tuple<DiscoveryRepresentation, ErrorRepresentation>> PatientFor(DiscoveryRequest request)
        {
            var patients = await matchingRepository.Where(request);
            var (patient, error) = DiscoveryUseCase.DiscoverPatient(filter.Do(patients, request).AsQueryable());
            if (patient == null)
            {
                return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(null, error);
            }
            await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                request.Patient.Id));
            var (linkRequest, exception) = await linkPatientRepository.GetLinkedCareContexts(request.Patient.Id);
            if (exception != null)
            {
                return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(null, new ErrorRepresentation(new Error(ErrorCode.FailedToGetLinkedCareContexts, "Failed to get Linked Care Contexts")));
            }
            if (linkRequest == null)
                return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(new DiscoveryRepresentation(patient), null);
            var unLinkedCareContexts = GetUnlinkedCareContexts(linkRequest, patient);
            patient.CareContexts = unLinkedCareContexts;
            return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(new DiscoveryRepresentation(patient), null);
        }

        private static IEnumerable<CareContextRepresentation> GetUnlinkedCareContexts(LinkRequest linkRequest, PatientEnquiryRepresentation patient)
        {
            return patient.CareContexts.Where(careContext =>
            {
                var linkedCareContexts = linkRequest.CareContexts
                    .Where(linkedCareContext => !linkedCareContext.CareContextNumber.Equals(careContext.ReferenceNumber));
                return linkedCareContexts.Any();
            });
        }
    }
}