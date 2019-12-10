using System;
using FluentAssertions;
using hip_library.Patient.models.dto;
using hip_service.Services;
using Xunit;

namespace hip_service_test.Services
{
    public class PatientDiscoveryServiceTest
    {
        [Fact]
        void shouldFilterPatientsFromJson()
        {
            var patientDiscoveryService = new PatientDiscoveryService("patients.json");
            var patients = patientDiscoveryService.GetPatients(new PatientRequest("9999999999", "xyz", "abc", "123456"));

            patients.Count.Should().Be(1);
            patients[0].Name.Should().Be("John Doe");
            patients[0].Gender.Should().Be("male");
            patients[0].ResourceType.Should().Be("patient");
            patients[0].BirthDate.Should().Be(new DateTime(2019,12,06));
            // Comparing Address field
            patients[0].Address.Use.Should().Be("home");
            patients[0].Address.City.Should().Be("Bengaluru");
            patients[0].Address.District.Should().Be("Bengaluru");
            patients[0].Address.Line.Should().Be("2, 3rd Cross, Jayanagar");
            patients[0].Address.State.Should().Be("Karnataka");
            patients[0].Address.PostalCode.Should().Be("560013");
            // Comparing the contact
            patients[0].Contact.Name.Should().Be("Jim");
            patients[0].Contact.Telecom.System.Should().Be("https://tmc.gov.in/ncg/telecom");
            patients[0].Contact.Telecom.Use.Should().Be("home");
            patients[0].Contact.Telecom.Value.Should().Be("7658765423");

            
        }
    }
} 