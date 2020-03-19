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
            var (linkRequest, exception) = await linkPatientRepository.GetLinkedCareContexts(request.Patient.Id);
            if (exception != null)
            {
                return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(null,
                    new ErrorRepresentation(new Error(ErrorCode.FailedToGetLinkedCareContexts,
                        "Failed to get Linked Care Contexts")));
            }

            if (linkRequest != null)
            {
                return await patientRepository.PatientWith(linkRequest.PatientReferenceNumber)
                    .Map(async patient =>
                    {
                        await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                            request.Patient.Id));
                        return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(
                            new DiscoveryRepresentation(patient.ToPatientEnquiryRepresentation(
                                GetUnlinkedCareContexts(linkRequest, patient))),
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

        private static IEnumerable<CareContextRepresentation> GetUnlinkedCareContexts(LinkRequest linkRequest,
            Patient patient)
        {
            return patient.CareContexts.Where(careContext =>
                linkRequest.CareContexts.Any(linkedCareContext =>
                    !linkedCareContext.CareContextNumber.Equals(careContext.ReferenceNumber)));
        }
    }
}