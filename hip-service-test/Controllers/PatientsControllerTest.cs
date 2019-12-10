using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using hip_library.Patient.models.domain;
using hip_library.Patient.models.dto;
using hip_service.Models.dto;
using hip_service.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace hip_service.Controllers
{
    public class PatientsControllerTest
    {
        [Fact]
        public void ShouldFilterPatientsFromJson()
        {
            var patientsController = new PatientsController(new PatientDiscoveryService("patients.json"));

            var patientsResponse =
                patientsController.Get(new PatientRequest("9999999999", "xyz", "abc", "123456")).Result as
                    OkObjectResult;

            patientsResponse.Should().NotBeNull();
            var expectedResponse = new PatientsResponse(new List<Patient>()
            {
                new Patient("patient", "John Doe", "male", new DateTime(2019, 12, 06),
                    new Address("home", "2, 3rd Cross, Jayanagar", "Bengaluru", "Bengaluru", "Karnataka", "560013"),
                    new Contact("Jim", new ContactPoint("home", "https://tmc.gov.in/ncg/telecom", "7658765423")))
            });
            
            patientsResponse.Value.Should().BeEquivalentTo(expectedResponse);
        }
    }
}