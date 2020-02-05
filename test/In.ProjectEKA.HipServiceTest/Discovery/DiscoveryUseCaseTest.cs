namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using HipLibrary.Patient.Model.Response;
    using HipService.Discovery;
    using Xunit;
    using Patient = HipLibrary.Patient.Model.Response.Patient;

    public class DiscoveryUseCaseTest
    {
        [Fact]
        private void ShouldReturnNoPatientFoundError()
        {
            var (patient, error) =
                DiscoveryUseCase.DiscoverPatient(new List<Patient>().AsQueryable());
            var expectedError = new ErrorResponse(new Error(ErrorCode.NoPatientFound, "No patient found"));

            patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private void ShouldReturnMultiplePatientsFoundError()
        {
            var patient1 = new Patient("123", "Jack", new List<CareContextRepresentation>(), new List<string>
            {
                Match.FIRST_NAME.ToString()
            });
            var patient2 = new Patient("123", "Jack", new List<CareContextRepresentation>(), new List<string>
            {
                Match.FIRST_NAME.ToString()
            });

            var (patient, error) =
                DiscoveryUseCase.DiscoverPatient(new List<Patient> { patient1, patient2}.AsQueryable());
            var expectedError = new ErrorResponse(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));

            patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private void ShouldReturnAPatient()
        {
            var patient1 = new Patient("123", "Jack", new List<CareContextRepresentation>(), new List<string>
            {
                Match.FIRST_NAME.ToString()
            });
            var (patient, error) =
                DiscoveryUseCase.DiscoverPatient(new List<Patient> { patient1 }.AsQueryable());

            error.Should().BeNull();
            patient.Should().BeEquivalentTo(patient1);
        }
    }
}