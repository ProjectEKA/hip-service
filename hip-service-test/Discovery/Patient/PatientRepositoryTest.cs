using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using hip_library.Patient.models;
using hip_service.Discovery.Patient;
using Xunit;

namespace hip_service_test.Discovery.Patient
{
    public class PatientRepositoryTest
    {
        [Fact]
        private async void ShouldGetPatientsBasedOnPhoneNumber()
        {
            var expectedPatient = new hip_library.Patient.models.Patient("", "", new List<CareContext>(), new List<string>());

            var patientRepository = new PatientRepository("patients.json");

            var patients = await ((IPatientRepository)patientRepository).SearchPatients("9999999999", null, null, null);

            patients.Count().Should().Be(1);
            AssertPatientDetails(patients.First(), expectedPatient);
        }

        [Fact]
        private async void ShouldNotGetPatientsIfStrongIdentifierDoesNotMatch()
        {
            var patientRepository = new PatientRepository("patients.json");

            var patients = await ((IPatientRepository)patientRepository).SearchPatients("45687747568645", null, null, null);

            patients.Count().Should().Be(0);
        }

        [Fact]
        private async void ShouldFilterBasedOnCaseReferenceNumberIfMatched()
        {
            var patientRepository = new PatientRepository("patients.json");

            var patients = await ((IPatientRepository)patientRepository).SearchPatients("45687747568645", null, null, null);

            patients.Count().Should().Be(0);
        }

        [Fact]
        private async void ShouldGetPatientsBasedOnCaseId()
        {
            var expectedPatient = new hip_library.Patient.models.Patient("", "", new List<CareContext>(), new List<string>());

            var patientRepository = new PatientRepository("patients.json");

            var patients = await ((IPatientRepository)patientRepository).SearchPatients("34071234", "123456", "xyz", "abc");

            patients.Count().Should().Be(1);
            AssertPatientDetails(patients.First(), expectedPatient);
        }

        [Fact]
        private async void ShouldGetPatientsBasedOnFirstName()
        {
            var expectedPatient = new hip_library.Patient.models.Patient("", "", new List<CareContext>(), new List<string>());

            var patientRepository = new PatientRepository("patients.json");

            var patients = await ((IPatientRepository)patientRepository).SearchPatients("34071234", "12345677677", "John", "abc");

            patients.Count().Should().Be(1);
            AssertPatientDetails(patients.First(), expectedPatient);
        }

        [Fact]
        private async void ShouldGetPatientsBasedOnLastName()
        {
            var expectedPatient = new hip_library.Patient.models.Patient("", "", new List<CareContext>(), new List<string>());

            var patientRepository = new PatientRepository("patients.json");

            var patients = await ((IPatientRepository)patientRepository).SearchPatients("34071234", "12345677677", "asdfa", "Doee");

            patients.Count().Should().Be(1);
            AssertPatientDetails(patients.First(), expectedPatient);
        }

        private static void AssertPatientDetails(hip_library.Patient.models.Patient patientActual, hip_library.Patient.models.Patient patientExpected)
        {
            patientActual.ReferenceNumber.Should().Be(patientExpected.ReferenceNumber);
            patientActual.Display.Should().Be(patientExpected.Display);
            patientActual.CareContexts.Should().AllBeEquivalentTo(patientExpected.CareContexts);
            patientActual.MatchedBy.Should().BeEquivalentTo(patientExpected.MatchedBy);
        }
    }
}