using System;
using System.Collections.Generic;
using FluentAssertions;
using hip_library.Patient.models;
using hip_service.Discovery.Patient;
using Xunit;

namespace hip_service_test.Discovery.Patient
{
    public class PatientDiscoveryTest
    {
        [Fact]
        private async void ShouldReturnPatient()
        {
            var patientMatchingRepository = new PatientMatchingRepository("patients.json");
            var patientDiscovery = new PatientDiscovery(patientMatchingRepository);

            var expectedPatient = new hip_library.Patient.models.Patient("1", "John Doee",
                new List<CareContextRepresentation>
                {
                    new CareContextRepresentation("123", "National Cancer program"),
                    new CareContextRepresentation("124", "National TB program")
                }, new List<string>
                {
                    "MOBILE",
                    "FirstName",
                    "Gender"
                });

            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.Mobile, "9999999999")
            };

            var unverifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.Mr, "123")
            };

            var discoveryRequest = new DiscoveryRequest(verifiedIdentifiers, unverifiedIdentifiers, "John", null,
                Gender.Male, new DateTime(2019, 01, 01));

            var (patient, error) = await patientDiscovery.PatientFor(discoveryRequest);


            patient.Should().BeEquivalentTo(expectedPatient);
            error.Should().BeNull();
        }

        [Fact]
        private async void ShouldGetMultiplePatientsFoundError()
        {
            var patientMatchingRepository = new PatientMatchingRepository("patients.json");
            var patientDiscovery = new PatientDiscovery(patientMatchingRepository);

            var expectedError = new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found");

            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.Mobile, "9999999999")
            };

            var discoveryRequest = new DiscoveryRequest(verifiedIdentifiers, new List<Identifier>(), null, null,
                Gender.Male, new DateTime(2019, 01, 01));

            var (patient, error) = await patientDiscovery.PatientFor(discoveryRequest);

            patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetNoPatientFoundError()
        {
            var patientMatchingRepository = new PatientMatchingRepository("patients.json");
            var patientDiscovery = new PatientDiscovery(patientMatchingRepository);

            var expectedError = new Error(ErrorCode.NoPatientFound, "No patient found");

            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.Mr, "311231231231")
            };

            var discoveryRequest = new DiscoveryRequest(verifiedIdentifiers, new List<Identifier>(), null, null,
                Gender.Male, new DateTime(2019, 01, 01));

            var (patient, error) = await patientDiscovery.PatientFor(discoveryRequest);

            patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }
    }
}