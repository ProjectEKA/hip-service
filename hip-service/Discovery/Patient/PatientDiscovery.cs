using System;
using System.Linq;
using System.Threading.Tasks;
using hip_service.Discovery.Patient.Matcher;
using HipLibrary.Patient;
using HipLibrary.Patient.Model.Request;
using HipLibrary.Patient.Model.Response;

namespace hip_service.Discovery.Patient
{
    using static StrongMatcherFactory;

    public class PatientDiscovery : IDiscovery
    {
        private readonly Filter filter;
        private readonly IMatchingRepository repo;
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;

        public PatientDiscovery(IMatchingRepository patientRepository,
            IDiscoveryRequestRepository discoveryRequestRepository)
        {
            repo = patientRepository;
            this.discoveryRequestRepository = discoveryRequestRepository;
            filter = new Filter();
        }

        public async Task<Tuple<DiscoveryResponse, ErrorResponse>> PatientFor(DiscoveryRequest request)
        {
            var expression = GetExpression(request.Patient.VerifiedIdentifiers);
            var patientInfos = await repo.Where(expression);
            var (patient, error) = DiscoveryUseCase.DiscoverPatient(filter.Do(patientInfos, request).AsQueryable());

            if (patient != null)
            {
                await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                    request.Patient.Id));
            }

            return new Tuple<DiscoveryResponse, ErrorResponse>(new DiscoveryResponse(patient), error);
        }
    }
}