namespace In.ProjectEKA.HipServiceTest.Discovery.Patient
{
    using System.Linq;
    using FluentAssertions;
    using HipService.Discovery.Patient;
    using Xunit;

    [Collection("Patient Repository Tests")]
    public class PatientMatchingRepositoryTest
    {
        [Fact]
        private async void ShouldReturnPatientsBasedOnExpression()
        {
            var patientMatchingRepository = new PatientMatchingRepository("patients.json");

            var patientInfo = await patientMatchingRepository.Where(patient => patient.PhoneNumber == "+919999999999");

            patientInfo.Count().Should().Be(4);
        }
    }
}