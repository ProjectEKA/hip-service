namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using Xunit;

    public class DiscoveryUseCaseTest
    {
        [Fact]
        private void ShouldReturnNoPatientFoundError()
        {
            var (patient, error) =
                DiscoveryUseCase.DiscoverPatient(new List<PatientEnquiryRepresentation>().AsQueryable());
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, "No patient found"));

            patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private void ShouldReturnMultiplePatientsFoundError()
        {
            var patient1 = new PatientEnquiryRepresentation("123", "Jack", new List<CareContextRepresentation>(), new List<string>
            {
                Match.Name.ToString()
            });
            var patient2 = new PatientEnquiryRepresentation("123", "Jack", new List<CareContextRepresentation>(), new List<string>
            {
                Match.Name.ToString()
            });

            var (patient, error) =
                DiscoveryUseCase.DiscoverPatient(new List<PatientEnquiryRepresentation> { patient1, patient2}.AsQueryable());
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));

            patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private void ShouldReturnAPatient()
        {
            var patient1 = new PatientEnquiryRepresentation("123", "Jack", new List<CareContextRepresentation>(), new List<string>
            {
                Match.Name.ToString()
            });
            var (patient, error) =
                DiscoveryUseCase.DiscoverPatient(new List<PatientEnquiryRepresentation> { patient1 }.AsQueryable());

            error.Should().BeNull();
            patient.Should().BeEquivalentTo(patient1);
        }
    }
}