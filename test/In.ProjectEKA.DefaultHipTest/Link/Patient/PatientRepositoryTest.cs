using In.ProjectEKA.DefaultHip.Link;

namespace In.ProjectEKA.DefaultHipTest.Link.Patient
{
    using FluentAssertions;
    using HipLibrary.Patient.Model;
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
    }
}