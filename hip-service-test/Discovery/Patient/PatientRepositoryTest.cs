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
            var expectedPatient = new hip_library.Patient.models.Patient("1", "John Doee", new List<CareContext>()
            {
                new CareContext("123", "National Cancer program"),
                new CareContext("124", "National TB program")
            }, new List<string>() {""});

            var patientRepository = new PatientRepository("patients.json");

            var patients =
                await ((IPatientRepository) patientRepository).SearchPatients("9999999999", null, null, null);

            patients.Count().Should().Be(1);
            AssertPatientDetails(patients.First(), expectedPatient);
        }

        [Fact]
        private async void ShouldNotGetPatientsIfStrongIdentifierDoesNotMatch()
        {
            var patientRepository = new PatientRepository("patients.json");

            var patients =
                await ((IPatientRepository) patientRepository).SearchPatients("45687747568645", null, null, null);

            patients.Count().Should().Be(0);
        }

        [Fact]
        private async void ShouldFilterBasedOnCaseReferenceNumberIfMatched()
        {
            var patientRepository = new PatientRepository("patients.json");

            var patients =
                await ((IPatientRepository) patientRepository).SearchPatients("45687747568645", null, null, null);

            patients.Count().Should().Be(0);
        }

        [Fact]
        private async void ShouldGetPatientsBasedOnCaseId()
        {
            var expectedPatient = new hip_library.Patient.models.Patient("1", "John Doee", new List<CareContext>()
            {
                new CareContext("123", "National Cancer program"),
            }, new List<string>() {""});

            var patientRepository = new PatientRepository("patients.json");

            var patients =
                await ((IPatientRepository) patientRepository).SearchPatients("9999999999", "123", null, null);

            patients.Count().Should().Be(1);
            AssertPatientDetails(patients.First(), expectedPatient);
        }

        [Fact]
        private async void ShouldGetPatientsBasedOnFirstName()
        {
            var expectedPatient = new hip_library.Patient.models.Patient("1", "John Doee", new List<CareContext>()
            {
                new CareContext("123", "National Cancer program"),
                new CareContext("124", "National TB program")
            }, new List<string>() {""});

            var patientRepository = new PatientRepository("patients.json");

            var patients =
                await ((IPatientRepository) patientRepository).SearchPatients("9999999999", null, "John", null);

            patients.Count().Should().Be(1);
            AssertPatientDetails(patients.First(), expectedPatient);
        }

        [Fact]
        private async void ShouldGetPatientsBasedOnLastName()
        {
            var expectedPatient = new hip_library.Patient.models.Patient("1", "John Doee", new List<CareContext>()
            {
                new CareContext("123", "National Cancer program"),
                new CareContext("124", "National TB program")
            }, new List<string>() {""});

            var patientRepository = new PatientRepository("patients.json");

            var patients =
                await ((IPatientRepository) patientRepository).SearchPatients("9999999999", null, null,
                    "Doee");

            patients.Count().Should().Be(1);
            AssertPatientDetails(patients.First(), expectedPatient);
        }

        private static void AssertPatientDetails(hip_library.Patient.models.Patient patientActual,
            hip_library.Patient.models.Patient patientExpected)
        {
            patientActual.ReferenceNumber.Should().Be(patientExpected.ReferenceNumber);
            patientActual.Display.Should().Be(patientExpected.Display);
            patientActual.CareContexts.Should().BeEquivalentTo(patientExpected.CareContexts);
            patientActual.MatchedBy.Should().BeEquivalentTo(patientExpected.MatchedBy);
        }
    }
}