using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using hip_library.Patient.models;
using hip_service.Discovery.Patient;
using hip_service.Discovery.Patient.Helpers;
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
                new Identifier(IdentifierType.Mobile, "9999999999")
            };

            var unverifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.Mr, "123")
            };

            var discoveryRequest = new DiscoveryRequest(verifiedIdentifiers, unverifiedIdentifiers, "John", null,
                Gender.Male, new DateTime(2019, 01, 01));

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
                new Identifier(IdentifierType.Mobile, "9999999999")
            };

            var discoveryRequest = new DiscoveryRequest(verifiedIdentifiers, new List<Identifier>(), null, null,
                Gender.Male, new DateTime(2019, 01, 01));

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
                new Identifier(IdentifierType.Mobile, "9999999999")
            };

            var discoveryRequest = new DiscoveryRequest(verifiedIdentifiers, new List<Identifier>(), null, null,
                Gender.Female, new DateTime(2019, 01, 01));

            var patients = FileReader.ReadJson("patients.json");

            var filteredPatients = filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }
    }
}