using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using hip_service.Discovery.Patient;
using HipLibrary.Patient.Model;
using HipLibrary.Patient.Model.Response;
using Xunit;

namespace hip_service_test.Discovery.Patient
{
    public class DiscoveryUseCaseTest
    {
        [Fact]
        private void ShouldReturnNoPatientFoundError()
        {
            var (patient, error) =
                DiscoveryUseCase.DiscoverPatient(new List<HipLibrary.Patient.Model.Response.Patient>().AsQueryable());
            var expectedError = new ErrorResponse(new Error(ErrorCode.NoPatientFound, "No patient found"));

            patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private void ShouldReturnMultiplePatientsFoundError()
        {
            var patient1 = new HipLibrary.Patient.Model.Response.Patient("123", "Jack", new List<CareContextRepresentation>(), new List<Match>
            {
                Match.FIRST_NAME
            });
            var patient2 = new HipLibrary.Patient.Model.Response.Patient("123", "Jack", new List<CareContextRepresentation>(), new List<Match>
            {
                Match.FIRST_NAME
            });

            var (patient, error) =
                DiscoveryUseCase.DiscoverPatient(new List<HipLibrary.Patient.Model.Response.Patient> { patient1, patient2}.AsQueryable());
            var expectedError = new ErrorResponse(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));

            patient.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private void ShouldReturnAPatient()
        {
            var patient1 = new HipLibrary.Patient.Model.Response.Patient("123", "Jack", new List<CareContextRepresentation>(), new List<Match>
            {
                Match.FIRST_NAME
            });
            var (patient, error) =
                DiscoveryUseCase.DiscoverPatient(new List<HipLibrary.Patient.Model.Response.Patient> { patient1 }.AsQueryable());

            error.Should().BeNull();
            patient.Should().BeEquivalentTo(patient1);
        }
    }
}