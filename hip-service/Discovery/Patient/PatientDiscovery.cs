using System;
using System.Linq;
using System.Threading.Tasks;
using hip_library.Patient;
using hip_library.Patient.models;
using hip_service.Discovery.Patient.Matcher;

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

        public async Task<Tuple<hip_library.Patient.models.Patient, Error>> PatientFor(DiscoveryRequest request)
        {
            var expression = GetExpression(request.VerifiedIdentifiers);
            var patientInfos = await repo.Where(expression);
            return DiscoveryUseCase.DiscoverPatient(filter.Do(patientInfos, request).AsQueryable());
        }
    }
}