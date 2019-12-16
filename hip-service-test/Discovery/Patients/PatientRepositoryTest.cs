using System;
using System.Linq;
using FluentAssertions;
using hip_library.Patient.models.domain;
using hip_service.Discovery.Patients;
using Xunit;

namespace hip_service_test.Discovery.Patients
{
    public class PatientRepositoryTest
    {
        [Fact]
        private void ShouldGetPatientsBasedOnPhoneNumber()
        {
            var patientRepository = new PatientRepository("patients.json");

            var patients = patientRepository.GetPatients("9999999999", "123456", "xyz", "abc").ToList();

            patients.Count.Should().Be(1);
            AssertPatientDetails(patients[0]);
        }

        [Fact]
        private void ShouldGetPatientsBasedOnCaseId()
        {
            var patientRepository = new PatientRepository("patients.json");

            var patients = patientRepository.GetPatients("34071234", "123456", "xyz", "abc").ToList();

            patients.Count.Should().Be(1);
            AssertPatientDetails(patients[0]);
        }

        [Fact]
        private void ShouldGetPatientsBasedOnFirstName()
        {
            var patientRepository = new PatientRepository("patients.json");

            var patients = patientRepository.GetPatients("34071234", "12345677677", "John", "abc").ToList();

            patients.Count.Should().Be(1);
            AssertPatientDetails(patients[0]);
        }

        [Fact]
        private void ShouldGetPatientsBasedOnLastName()
        {
            var patientRepository = new PatientRepository("patients.json");

            var patients = patientRepository.GetPatients("34071234", "12345677677", "asdfa", "Doee").ToList();

            patients.Count.Should().Be(1);
            AssertPatientDetails(patients[0]);
        }

        private static void AssertPatientDetails(Patient patient)
        {
            patient.Name.Should().Be("John Doee");
            patient.Gender.Should().Be("male");
            patient.ResourceType.Should().Be("patient");
            patient.BirthDate.Should().Be(new DateTime(2019, 12, 06));
            // Comparing Address field
            patient.Address.Use.Should().Be("home");
            patient.Address.City.Should().Be("Bengaluru");
            patient.Address.District.Should().Be("Bengaluru");
            patient.Address.Line.Should().Be("2, 3rd Cross, Jayanagar");
            patient.Address.State.Should().Be("Karnataka");
            patient.Address.PostalCode.Should().Be("560013");
            // Comparing the contact
            patient.Contact.Name.Should().Be("Jim");
            patient.Contact.Telecom.System.Should().Be("https://tmc.gov.in/ncg/telecom");
            patient.Contact.Telecom.Use.Should().Be("home");
            patient.Contact.Telecom.Value.Should().Be("7658765423");
        }
    }
}