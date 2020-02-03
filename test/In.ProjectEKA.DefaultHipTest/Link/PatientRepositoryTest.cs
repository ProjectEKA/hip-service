using In.ProjectEKA.DefaultHip.Link;

namespace In.ProjectEKA.DefaultHipTest.Link
{
    using FluentAssertions;
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
            var testPatient = new HipLibrary.Patient.Model.Patient
            {
                PhoneNumber = "+919999999999",
                Identifier = patientReferenceNumber,
                FirstName = "Jill",
                LastName = "Doee",
                Gender = "F",
            };
            
            var patient = patientRepository
                .PatientWith(patientReferenceNumber);
            
            patient.ValueOr(new HipLibrary.Patient.Model.Patient()).Should()
                .BeEquivalentTo(testPatient);
        }
        
        [Fact]
        private void ReturnNullForUnknownPatient()
        {
            const string patientReferenceNumber = "1234";
            
            var patient = patientRepository
                .PatientWith(patientReferenceNumber);
            
            patient.ValueOr((HipLibrary.Patient.Model.Patient) null)
                .Should().BeNull();
        }
    }
}