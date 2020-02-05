namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;

    public class PatientDiscovery : IDiscovery
    {
        private readonly Filter filter;
        private readonly IMatchingRepository matchingRepository;
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;

        public PatientDiscovery(
            IMatchingRepository patientRepository,
            IDiscoveryRequestRepository discoveryRequestRepository)
        {
            matchingRepository = patientRepository;
            this.discoveryRequestRepository = discoveryRequestRepository;
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
            return new Tuple<DiscoveryRepresentation, ErrorRepresentation>(new DiscoveryRepresentation(patient), null);
        }
    }
}