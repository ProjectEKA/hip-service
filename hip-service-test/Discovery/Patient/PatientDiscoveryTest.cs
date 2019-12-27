using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using hip_library.Patient.models;
using hip_service.Discovery.Patient;
using hip_service.Discovery.Patients;
using Moq;
using Xunit;

namespace hip_service_test.Discovery.Patient
{
    public class PatientDiscoveryTest
    {
        [Fact]
        private async void ShouldReturnPatient()
        {
            var mockPatientRepository = new Mock<IPatientRepository>();
            var patientDiscovery = new PatientDiscovery(mockPatientRepository.Object, new DiscoveryUseCase());

            var patient1 = new hip_library.Patient.models.Patient("123", "xyz abc", new List<CareContextRepresentation>
            {
                new CareContextRepresentation("321", "display")
            }, new List<string>
            {
                "name"
            });
            var patients = new List<hip_library.Patient.models.Patient> {patient1}.AsQueryable();
            mockPatientRepository
                .Setup(x => x.SearchPatients("9999999999", "123456", "xyz", "abc"))
                .ReturnsAsync(patients);
            
            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.Mobile, "9999999999")
            };

            var unverifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.Mr, "123456")
            };

            var discoveryRequest = new DiscoveryRequest(verifiedIdentifiers, unverifiedIdentifiers, "xyz", "abc",
                Gender.Male, new DateTime(2019, 01, 01));

            var (patient, error) = await patientDiscovery.PatientFor(discoveryRequest);

            mockPatientRepository.Verify();

            patient.Should().BeEquivalentTo(patient1);
            error.Should().BeNull();
        }

        [Fact]
        private async void ShouldGetMultiplePatientsFoundError()
        {
            var mockPatientRepository = new Mock<IPatientRepository>();
            var patientDiscovery = new PatientDiscovery(mockPatientRepository.Object, new DiscoveryUseCase());

            var patient1 = new hip_library.Patient.models.Patient("123", "xyz abc", new List<CareContextRepresentation>
            {
                new CareContextRepresentation("321", "display")
            }, new List<string>
            {
                "name"
            });
            var patient2 = new hip_library.Patient.models.Patient("124", "xyz eee", new List<CareContextRepresentation>
            {
                new CareContextRepresentation("322", "display")
            }, new List<string>
            {
                "name"
            });
            var patients = new List<hip_library.Patient.models.Patient> {patient1, patient2}.AsQueryable();
            mockPatientRepository
                .Setup(x => x.SearchPatients("9999999999", "123456", "xyz", "abc"))
                .ReturnsAsync(patients);

            var expectedError = new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found");

            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.Mobile, "9999999999")
            };

            var unverifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.Mr, "123456")
            };

            var discoveryRequest = new DiscoveryRequest(verifiedIdentifiers, unverifiedIdentifiers, "xyz", "abc",
                Gender.Male, new DateTime(2019, 01, 01));

            var (patient, error) = await patientDiscovery.PatientFor(discoveryRequest);

            mockPatientRepository.Verify();

            patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }
    }
}