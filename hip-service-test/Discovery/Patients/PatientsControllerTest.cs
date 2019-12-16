using System;
using System.Collections.Generic;
using FluentAssertions;
using hip_library.Patient;
using hip_library.Patient.models.domain;
using hip_library.Patient.models.dto;
using hip_service.Discovery.Patients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace hip_service.Controllers
{
    public class PatientsControllerTest
    {
        [Fact]
        public void ShouldCallPatientsDiscovery()
        {
            var patients = new List<Patient>
            {
                new Patient("patient", "John Doe", "male", new DateTime(2019, 12, 06),
                    new Address("home", "2, 3rd Cross, Jayanagar", "Bengaluru", "Bengaluru", "Karnataka", "560013"),
                    new Contact("Jim", new ContactPoint("home", "https://tmc.gov.in/ncg/telecom", "7658765423")))
            };
            var expectedResponse = new PatientsResponse(patients);

            var patientRequest = new PatientRequest("9999999999", "xyz", "abc", "123456");

            var mockPatientDiscovery = new Mock<IDiscovery>();
            mockPatientDiscovery.Setup(x => x.GetPatients(patientRequest)).Returns(patients);

            var patientsController = new PatientsController(mockPatientDiscovery.Object);
            var response =
                patientsController.Get(patientRequest).Result as
                    OkObjectResult;

            mockPatientDiscovery.Verify();
            response.Should().NotBeNull();
            response.Value.Should().BeEquivalentTo(expectedResponse);
        }
    }
}