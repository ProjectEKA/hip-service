namespace In.ProjectEKA.DefaultHipTest.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DefaultHip.Discovery;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using Xunit;

    [Collection("Patient Repository Tests")]
    public class PatientMatchingRepositoryTest
    {
        [Fact]
        private async void ShouldReturnPatientsBasedOnExpression()
        {
            var patientMatchingRepository = new PatientMatchingRepository("patients.json");
            var phoneNumberIdentifier = new Identifier(IdentifierType.MOBILE, "+91-9605417827");
            var request = new DiscoveryRequest(
                new PatientEnquiry(string.Empty,
                    new List<Identifier> {phoneNumberIdentifier},
                    null,
                    string.Empty,
                    string.Empty,
                    Gender.F,
                    DateTime.Now),
                string.Empty);

            var patientInfo = await patientMatchingRepository.Where(request);

            patientInfo.Count().Should().Be(1);
        }
    }
}