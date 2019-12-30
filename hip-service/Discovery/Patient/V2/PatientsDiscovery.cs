namespace hip_service.Discovery.Patients
{
    using System;
    using System.Threading.Tasks;
    using hip_library.Patient;
    using static Patient.StrongMatcherFactory;
    using hip_library.Patient.models;
    using Patient;

    public class PatientsDiscovery : IDiscovery
    {
        private readonly Filter filter;
        private readonly DiscoveryUseCase discoveryUseCase;
        private readonly IMatchingRepository repo;

        public PatientsDiscovery(IMatchingRepository patientRepository, DiscoveryUseCase discoveryUseCase)
        {
            repo = patientRepository;
            filter = new Filter();
            this.discoveryUseCase = discoveryUseCase;
        }

        public async Task<Tuple<Patient, Error>> PatientFor(DiscoveryRequest request)
        {
            var expression = GetExpression(request.VerifiedIdentifiers);
            var patientInfos = await repo.Where(expression);
            return discoveryUseCase.DiscoverPatient(filter.Do(patientInfos, request));
        }
    }
}