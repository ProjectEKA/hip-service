using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using hip_library.Patient.models;
using hip_service.Discovery.Patient;
using Xunit;

namespace hip_service_test.Discovery.Patient
{
    public class DiscoveryUseCaseTest
    {
        [Fact]
        private void ShouldReturnNoPatientFoundError()
        {
            var (patient, error) =
                DiscoveryUseCase.DiscoverPatient(new List<hip_library.Patient.models.Patient>().AsQueryable());
            var expectedError = new Error(ErrorCode.NoPatientFound, "No patient found");

            patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private void ShouldReturnMultiplePatientsFoundError()
        {
            var patient1 = new hip_library.Patient.models.Patient("123", "Jack", new List<CareContextRepresentation>(), new List<string>());
            var patient2 = new hip_library.Patient.models.Patient("123", "Jack", new List<CareContextRepresentation>(), new List<string>());

            var (patient, error) =
                DiscoveryUseCase.DiscoverPatient(new List<hip_library.Patient.models.Patient> { patient1, patient2}.AsQueryable());
            var expectedError = new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found");

            patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private void ShouldReturnAPatient()
        {
            var patient1 = new hip_library.Patient.models.Patient("123", "Jack", new List<CareContextRepresentation>(), new List<string>());

        }
    }
}