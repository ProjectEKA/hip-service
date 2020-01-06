using System;
using System.Collections.Generic;
using FluentAssertions;
using hip_service.Discovery.Patient;
using HipLibrary.Patient;
using HipLibrary.Patient.Models;
using HipLibrary.Patient.Models.Request;
using HipLibrary.Patient.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace hip_service_test.Discovery.Patient
{
    public class PatientsControllerTest
    {
        [Fact]
        public async void ShouldGetAPatient()
        {
            var verifiedIdentifiers = new List<Identifier>()
            {
                new Identifier(IdentifierType.MOBILE, "9999999999")
            };

            var unverifiedIdentifiers = new List<Identifier>()
            {
                new Identifier(IdentifierType.MR, "1")
            };

            var patientRequest = new HipLibrary.Patient.Models.Request.Patient("cm-1",verifiedIdentifiers,
                unverifiedIdentifiers, "J", "K",
                Gender.M, new DateTime(2019, 01, 01));

            var discoveryRequest = new DiscoveryRequest(patientRequest);

            var mockDiscovery = new Mock<IDiscovery>();
            var expectedPatient = new HipLibrary.Patient.Models.Response.Patient("p1", "J K", new List<CareContextRepresentation>()
            {
                new CareContextRepresentation("1", "display")
            }, new List<HipLibrary.Patient.Models.Response.Match>());
            var expectedResponse = new DiscoveryResponse(expectedPatient);

            mockDiscovery.Setup(x => x.PatientFor(discoveryRequest)).ReturnsAsync(
                new Tuple<DiscoveryResponse, Error>(expectedResponse, null));

            var accountsController = new PatientsController(mockDiscovery.Object);
            var response = accountsController.Discover(discoveryRequest).Result as OkObjectResult;

            mockDiscovery.Verify();
            response.Should().NotBeNull();
            response.Value.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public void ShouldGetNotFound()
        {
            var verifiedIdentifiers = new List<Identifier>()
            {
                new Identifier(IdentifierType.MOBILE, "9999999999")
            };

            var unverifiedIdentifiers = new List<Identifier>()
            {
                new Identifier(IdentifierType.MR, "1")
            };
            var patient = new HipLibrary.Patient.Models.Request.Patient("cm-1", verifiedIdentifiers, unverifiedIdentifiers, "J", "K",
                Gender.M, new DateTime(2019, 01, 01));

            var discoveryRequest = new DiscoveryRequest(patient);

            var mockDiscovery = new Mock<IDiscovery>();
            var error = new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found");
            mockDiscovery.Setup(x => x.PatientFor(discoveryRequest)).ReturnsAsync(
                new Tuple<DiscoveryResponse, Error>(null, error));

            var accountsController = new PatientsController(mockDiscovery.Object);
            var response = accountsController.Discover(discoveryRequest).Result as NotFoundObjectResult;

            mockDiscovery.Verify();
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}