namespace In.ProjectEKA.DefaultHipTest.Link
{
    using DefaultHip.Link;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using Xunit;

    [Collection("Patient Repository Tests")]
    public class PatientRepositoryTest
    {
        private readonly PatientRepository patientRepository = new PatientRepository("demoPatients.json");

        [Fact]
        private async System.Threading.Tasks.Task ReturnPatientAsync()
        {
            const string patientReferenceNumber = "RVH1002";
            var testPatient = new Patient
            {
                PhoneNumber = "+91-7777777777",
                Identifier = patientReferenceNumber,
                Gender = Gender.F,
                Name = "Navjot Singh",
                CareContexts = new []
                {
                    new CareContextRepresentation("NCP1007", "National Cancer program"),
                    new CareContextRepresentation("RV-MHD-01.17.0024", "Dept of Psychiatry - Episode 1"),
                },
                YearOfBirth = 2001
            };

            var patient = await patientRepository.PatientWithAsync(patientReferenceNumber);

            patient.ValueOr(new Patient()).Should().BeEquivalentTo(testPatient);
        }

        [Fact]
        private async System.Threading.Tasks.Task ReturnNullForUnknownPatientAsync()
        {
            const string patientReferenceNumber = "1234";

            var patient = await patientRepository.PatientWithAsync(patientReferenceNumber);

            patient.ValueOr((Patient) null).Should().BeNull();
        }
    }
}