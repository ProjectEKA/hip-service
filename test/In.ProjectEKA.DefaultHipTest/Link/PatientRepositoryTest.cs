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
            const string patientReferenceNumber = "1";
            var testPatient = new Patient
            {
                PhoneNumber = "+91-9605417827",
                Identifier = patientReferenceNumber,
                FirstName = "Anushree",
                LastName = "Raman",
                Gender = "F",
                CareContexts = new []
                {
                    new CareContextRepresentation("124", "National TB program"),
                    new CareContextRepresentation("123", "National Cancer program"), 
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