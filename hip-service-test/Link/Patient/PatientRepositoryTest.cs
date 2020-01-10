using System;
using FluentAssertions;
using hip_service.Discovery.Patient.Model;
using hip_service.Link.Patient;
using Xunit;

namespace hip_service_test.Link.Patient
{
    [Collection("Patient Repository Tests")]
    public class PatientRepositoryTest
    {
        private readonly PatientRepository patientRepository 
            = new PatientRepository("patients.json");
        
        [Fact]
        private void ShouldNotReturnNullPatient()
        {
            const string patientReferenceNumber = "11";
            var patient = patientRepository
                .GetPatientInfoWithReferenceNumber(patientReferenceNumber);
            var mockPatient = new hip_service.Discovery.Patient.Model.Patient
            {
                PhoneNumber = "+919999999999",
                Identifier = "11",
                FirstName = "Jill",
                LastName = "Doee",
                Gender = "F",
                DateOfBirth = DateTime.ParseExact("2019-12-06","yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture),
                Email = "zzy@def.com",
            };

            patient.ValueOr(new hip_service.Discovery.Patient.Model.Patient()).Should()
                .BeEquivalentTo(mockPatient);
        }
        
        [Fact]
        private void ShouldReturnNullPatient()
        {
            const string patientReferenceNumber = "1234";
            var patient = patientRepository
                .GetPatientInfoWithReferenceNumber(patientReferenceNumber);
            patient.HasValue.Should().BeFalse();
        }
        
        [Fact]
        private void ShouldReturnNotNullCareContext()
        {
            const string patientReferenceNumber = "4";
            const string careContextReferenceNumber = "129";
            var careContext = new CareContext
            {
                ReferenceNumber = careContextReferenceNumber,
                Description = "National Cancer program"
            };
            patientRepository.GetProgramInfo(patientReferenceNumber,
                    careContextReferenceNumber).ValueOr(new CareContext())
                .Should().BeEquivalentTo(careContext);
        }
        
        [Fact]
        private void ShouldReturnNullCareContext()
        {
            const string patientReferenceNumber = "4";
            const string careContextReferenceNumber = "190";
            patientRepository.GetProgramInfo(patientReferenceNumber,
                careContextReferenceNumber).HasValue.Should().BeFalse();
        }
    }
}