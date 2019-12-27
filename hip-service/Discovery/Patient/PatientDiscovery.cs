using System;
using System.Linq;
using System.Threading.Tasks;
using hip_library.Patient;
using hip_library.Patient.models;

namespace hip_service.Discovery.Patients
{
    public class PatientDiscovery : IDiscovery
    {
        private readonly Patient.IPatientRepository patientRepository;
        private readonly Patient.DiscoveryUseCase discoveryUseCase;

        public PatientDiscovery(Patient.IPatientRepository patientRepository, Patient.DiscoveryUseCase discoveryUseCase)
        {
            this.patientRepository = patientRepository;
            this.discoveryUseCase = discoveryUseCase;
        }

        public async Task<Tuple<hip_library.Patient.models.Patient, Error>> PatientFor(DiscoveryRequest request)
        {
            var patients = await patientRepository.SearchPatients(GetPhoneNumberFrom(request),
                GetCaseReferenceNumberFrom(request), request.FirstName, request.LastName);
            
            return discoveryUseCase.DiscoverPatient(patients);
        }

        private static string GetPhoneNumberFrom(DiscoveryRequest request)
        {
            return request.VerifiedIdentifiers.FirstOrDefault(identifier => identifier.Type == IdentifierType.Mobile)
                ?.Value;
        }

        private static string GetCaseReferenceNumberFrom(DiscoveryRequest request)
        {
            return request.UnverifiedIdentifiers.FirstOrDefault(identifier => identifier.Type == IdentifierType.Mr)
                ?.Value;
        }

    }
}