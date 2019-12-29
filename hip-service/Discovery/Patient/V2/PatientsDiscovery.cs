namespace hip_service.Discovery.Patients
{
    using System;
    using System.Threading.Tasks;
    using hip_library.Patient;
    using hip_library.Patient.models;

    public class PatientsDiscovery : IDiscovery
    {
        private readonly Discovery.Patient.Filter filter;
        private readonly Discovery.Patient.DiscoveryUseCase discoveryUseCase;

        public PatientsDiscovery(Discovery.Patient.IMatchingRepository patientRepository, Discovery.Patient.DiscoveryUseCase discoveryUseCase)
        {
            filter = new Discovery.Patient.Filter(patientRepository);
            this.discoveryUseCase = discoveryUseCase;
        }

        public async Task<Tuple<Patient, Error>> PatientFor(DiscoveryRequest request)
        {
            var patients = await filter.DoFilter(request);

            return discoveryUseCase.DiscoverPatient(patients);
        }
    }
}