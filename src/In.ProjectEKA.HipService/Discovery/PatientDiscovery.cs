namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model.Response;
    using DiscoveryRequest = HipLibrary.Patient.Model.Request.DiscoveryRequest;

    public class PatientDiscovery : IDiscovery
    {
        private readonly Filter filter;
        private readonly IMatchingRepository matchingRepository;
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;

        public PatientDiscovery(IMatchingRepository patientRepository,
            IDiscoveryRequestRepository discoveryRequestRepository)
        {
            matchingRepository = patientRepository;
            this.discoveryRequestRepository = discoveryRequestRepository;
            filter = new Filter();
        }

        public async Task<Tuple<DiscoveryResponse, ErrorResponse>> PatientFor(DiscoveryRequest request)
        {
            var patientInfos = await matchingRepository.Where(request);
            var (patient, error) = DiscoveryUseCase.DiscoverPatient(filter.Do(patientInfos, request).AsQueryable());

            if (patient == null)
            {
                return new Tuple<DiscoveryResponse, ErrorResponse>(null, error);
            }

            await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                request.Patient.Id));
            return new Tuple<DiscoveryResponse, ErrorResponse>(new DiscoveryResponse(patient), null);
        }
    }
}