using System;
using System.Linq;
using System.Threading.Tasks;
using hip_service.Discovery.Patient.Matcher;
using HipLibrary.Patient;
using HipLibrary.Patient.Models.Request;
using HipLibrary.Patient.Models.Response;

namespace hip_service.Discovery.Patient
{
    using static StrongMatcherFactory;

    public class PatientDiscovery : IDiscovery
    {
        private readonly Filter filter;
        private readonly IMatchingRepository repo;

        public PatientDiscovery(IMatchingRepository patientRepository)
        {
            repo = patientRepository;
            filter = new Filter();
        }

        public async Task<Tuple<DiscoveryResponse, ErrorResponse>> PatientFor(DiscoveryRequest request)
        {
            var expression = GetExpression(request.Patient.VerifiedIdentifiers);
            var patientInfos = await repo.Where(expression);
            var (patient, error) = DiscoveryUseCase.DiscoverPatient(filter.Do(patientInfos, request).AsQueryable());

            return new Tuple<DiscoveryResponse, ErrorResponse>(
            new DiscoveryResponse(patient),
                error);
        }
    }
}
