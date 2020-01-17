namespace In.ProjectEKA.HipService.Discovery.Patient
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model.Request;
    using HipLibrary.Patient.Model.Response;
    using static Matcher.StrongMatcherFactory;

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