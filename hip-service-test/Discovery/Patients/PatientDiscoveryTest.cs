using System;
using System.Collections.Generic;
using hip_library.Patient.models.domain;
using hip_library.Patient.models.dto;
using hip_service.Discovery.Patients;
using Moq;
using Xunit;

namespace hip_service_test.Services
{
    public class PatientDiscoveryTest
    {
        [Fact]
        private void ShouldCallPatientRepository()
        {
            var mockPatientRepository = new Mock<IPatientRepository>();
            var patientDiscoveryService = new PatientDiscovery(mockPatientRepository.Object);

            mockPatientRepository
                .Setup(x => x.GetPatients("9999999999", "123456", "xyz", "abc"))
                .Returns(new List<Patient>()
                {
                    new Patient("patient", "John Doe", "male", new DateTime(2019, 12, 06),
                        new Address("home", "2, 3rd Cross, Jayanagar", "Bengaluru", "Bengaluru", "Karnataka", "560013"),
                        new Contact("Jim", new ContactPoint("home", "https://tmc.gov.in/ncg/telecom", "7658765423")))
                });

            var patients = patientDiscoveryService.GetPatients(new PatientRequest("9999999999", "xyz", "abc", "123456"));

            mockPatientRepository.Verify();
        }
    }
}