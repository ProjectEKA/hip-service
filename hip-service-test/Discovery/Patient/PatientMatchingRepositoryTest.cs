using System.Linq;
using FluentAssertions;
using hip_service.Discovery.Patient;
using Xunit;

namespace hip_service_test.Discovery.Patient
{
    public class PatientMatchingRepositoryTest
    {
        [Fact]
        private async void ShouldReturnPatientsBasedOnExpression()
        {
            var patientMatchingRepository = new PatientMatchingRepository("patients.json");

            var patientInfo = await patientMatchingRepository.Where(patient => patient.PhoneNumber == "9999999999");

            patientInfo.Count().Should().Be(3);
        }
    }
}