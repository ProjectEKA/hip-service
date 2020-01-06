using System;
using System.Collections.Generic;
using FluentAssertions;
using hip_service.Discovery.Patient;
using HipLibrary.Patient.Models;
using HipLibrary.Patient.Models.Request;
using HipLibrary.Patient.Models.Response;
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

            var expectedPatient = new HipLibrary.Patient.Models.Response.Patient("1", "John Doee",
                new List<CareContextRepresentation>
                {
                    new CareContextRepresentation("123", "National Cancer program"),
                    new CareContextRepresentation("124", "National TB program")
                }, new List<Match>
                {
                    Match.MOBILE,
                    Match.FIRST_NAME,
                    Match.GENDER
                });

            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MOBILE, "9999999999")
            };

            var unverifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MR, "123")
            };

            var patientRequest = new HipLibrary.Patient.Models.Request.Patient("cm-1", verifiedIdentifiers,
                unverifiedIdentifiers, "John", null, Gender.M, new DateTime(2019, 01, 01));

            var discoveryRequest = new DiscoveryRequest(patientRequest);

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);


            discoveryResponse.Patient.Should().BeEquivalentTo(expectedPatient);
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
                new Identifier(IdentifierType.MOBILE, "9999999999")
            };

            var patientRequest = new HipLibrary.Patient.Models.Request.Patient("cm-1", verifiedIdentifiers,
                new List<Identifier>(), null, null, Gender.M, new DateTime(2019, 01, 01));
            var discoveryRequest = new DiscoveryRequest(patientRequest);

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Patient.Should().BeNull();
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
                new Identifier(IdentifierType.MR, "311231231231")
            };

            var patientRequest = new HipLibrary.Patient.Models.Request.Patient("cm-1", verifiedIdentifiers,
                new List<Identifier>(), null, null,
                Gender.M, new DateTime(2019, 01, 01));

            var discoveryRequest = new DiscoveryRequest(patientRequest);

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }
    }
}