namespace In.ProjectEKA.HipServiceTest.Link.Patient
{
    using System;
    using FluentAssertions;
    using HipService.Discovery.Patient.Model;
    using HipService.Link.Patient;
    using Xunit;

    [Collection("Patient Repository Tests")]
    public class PatientRepositoryTest
    {
        private readonly PatientRepository patientRepository 
            = new PatientRepository("patients.json");
        
        [Fact]
        private void ReturnObjectForKnownPatient()
        {
            const string patientReferenceNumber = "11";
            var testPatient = new Patient
            {
                PhoneNumber = "+919999999999",
                Identifier = patientReferenceNumber,
                FirstName = "Jill",
                LastName = "Doee",
                Gender = "F",
                DateOfBirth = DateTime.ParseExact("2019-12-06","yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture),
                Email = "zzy@def.com",
            };
            
            var patient = patientRepository
                .PatientWith(patientReferenceNumber);
            
            patient.ValueOr(new Patient()).Should()
                .BeEquivalentTo(testPatient);
        }
        
        [Fact]
        private void ReturnNullForUnknownPatient()
        {
            const string patientReferenceNumber = "1234";
            
            var patient = patientRepository
                .PatientWith(patientReferenceNumber);
            
            patient.ValueOr((Patient) null)
                .Should().BeNull();
        }
        
        [Fact]
        private void ReturnNotNullCareContextForValidPatient()
        {
            const string patientReferenceNumber = "4";
            const string careContextReferenceNumber = "129";
            var careContext = new CareContext
            {
                ReferenceNumber = careContextReferenceNumber,
                Description = "National Cancer program"
            };
            
            patientRepository.ProgramInfoWith(patientReferenceNumber,
                    careContextReferenceNumber).ValueOr(new CareContext())
                .Should().BeEquivalentTo(careContext);
        }
        
        [Fact]
        private void ReturnNullCareContextForValidPatient()
        {
            const string patientReferenceNumber = "4";
            const string careContextReferenceNumber = "190";
            
            patientRepository.ProgramInfoWith(patientReferenceNumber,
                careContextReferenceNumber).ValueOr((CareContext)null)
                .Should().BeNull();
        }
    }
}