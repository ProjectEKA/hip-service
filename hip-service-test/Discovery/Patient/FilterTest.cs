using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using hip_service.Discovery.Patient;
using hip_service.Discovery.Patient.Helpers;
using HipLibrary.Patient.Models;
using HipLibrary.Patient.Models.Request;
using Xunit;

namespace hip_service_test.Discovery.Patient
{
    public class FilterTest
    {
        [Fact]
        private void ShouldFilterAndReturnAPatientByUnverifiedIdentifier()
        {
            var filter = new Filter();

            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MOBILE, "9999999999")
            };

            var unverifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MR, "123")
            };

            var patient = new HipLibrary.Patient.Models.Request.Patient("cm-123", verifiedIdentifiers,
                unverifiedIdentifiers, "John", null, Gender.M, new DateTime(2019, 01, 01));

            var discoveryRequest = new DiscoveryRequest(patient, "transaction-id-1");

            var patients = FileReader.ReadJson("patients.json");

            var filteredPatients = filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }

        [Fact]
        private void ShouldFilterAndReturnMultiplePatientsByPhoneNumber()
        {
            var filter = new Filter();

            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MOBILE, "9999999999")
            };

            var patient = new HipLibrary.Patient.Models.Request.Patient("cm-1", verifiedIdentifiers,
                new List<Identifier>(), null, null,
                Gender.M, new DateTime(2019, 01, 01));

            var discoveryRequest = new DiscoveryRequest(patient, "transaction-id-1");

            var patients = FileReader.ReadJson("patients.json");

            var filteredPatients = filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(2);
        }

        [Fact]
        private void ShouldFilterAndReturnMultiplePatientsByGender()
        {
            var filter = new Filter();

            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MOBILE, "9999999999")
            };

            var patient = new HipLibrary.Patient.Models.Request.Patient("cm-1", verifiedIdentifiers,
                new List<Identifier>(), null, null, Gender.F, new DateTime(2019, 01, 01));

            var discoveryRequest = new DiscoveryRequest(patient, "transaction-id-1");

            var patients = FileReader.ReadJson("patients.json");

            var filteredPatients = filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }
    }
}