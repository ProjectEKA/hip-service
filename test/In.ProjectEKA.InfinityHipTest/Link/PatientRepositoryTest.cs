namespace In.ProjectEKA.DefaultHipTest.Link
{
    using DefaultHip.Link;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using Xunit;

    [Collection("Patient Repository Tests")]
    public class PatientRepositoryTest
    {
        private readonly PatientRepository patientRepository = new PatientRepository("patients.json");
        
        [Fact]
        private void ReturnObjectForKnownPatient()
        {
            const string patientReferenceNumber = "3423";
            var testPatient = new Patient
            {
                PhoneNumber = "+91-9036346499",
                Identifier = patientReferenceNumber,
                FirstName = "Sridhar",
                LastName = "Kalagi",
                Gender = "M",
                CareContexts = new []
                {
                    new CareContextRepresentation("TBAccount-123", "TB Account"),
                    new CareContextRepresentation("Diabetes-11", "Diabetes Program"), 
                }
            };
            
            var patient = patientRepository.PatientWith(patientReferenceNumber);
            
            patient.ValueOr(new Patient()).Should().BeEquivalentTo(testPatient);
        }
        
        [Fact]
        private void ReturnNullForUnknownPatient()
        {
            const string patientReferenceNumber = "1234";
            
            var patient = patientRepository.PatientWith(patientReferenceNumber);
            
            patient.ValueOr((Patient) null).Should().BeNull();
        }
    }
}