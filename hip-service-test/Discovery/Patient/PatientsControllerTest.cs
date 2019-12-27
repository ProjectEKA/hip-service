using System;
using System.Collections.Generic;
using FluentAssertions;
using hip_library.Patient;
using hip_library.Patient.models;
using hip_service.Discovery.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace hip_service_test.Discovery.Patient
{
    public class PatientsControllerTest
    {
        [Fact]
        public void ShouldGetAPatient()
        {
            var verifiedIdentifiers = new List<Identifier>()
            {
                new Identifier(IdentifierType.Mobile, "9999999999")
            };

            var unverifiedIdentifiers = new List<Identifier>()
            {
                new Identifier(IdentifierType.Mr, "1")
            };

            var discoveryRequest = new DiscoveryRequest(verifiedIdentifiers, unverifiedIdentifiers, "J", "K",
                Gender.Male, new DateTime(2019, 01, 01));

            var mockDiscovery = new Mock<IDiscovery>();
            var expectedPatient = new hip_library.Patient.models.Patient("p1", "J K", new List<CareContextRepresentation>()
            {
                new CareContextRepresentation("1", "display")
            }, new List<string>());
            mockDiscovery.Setup(x => x.PatientFor(discoveryRequest)).ReturnsAsync(
                new Tuple<hip_library.Patient.models.Patient, Error>(expectedPatient, null));

            var accountsController = new PatientController(mockDiscovery.Object);
            var response = accountsController.Discover(discoveryRequest).Result as OkObjectResult;

            mockDiscovery.Verify();
            response.Should().NotBeNull();
            response.Value.Should().BeEquivalentTo(expectedPatient);
        }

        [Fact]
        public void ShouldGetNotFound()
        {
            var verifiedIdentifiers = new List<Identifier>()
            {
                new Identifier(IdentifierType.Mobile, "9999999999")
            };

            var unverifiedIdentifiers = new List<Identifier>()
            {
                new Identifier(IdentifierType.Mr, "1")
            };

            var discoveryRequest = new DiscoveryRequest(verifiedIdentifiers, unverifiedIdentifiers, "J", "K",
                Gender.Male, new DateTime(2019, 01, 01));

            var mockDiscovery = new Mock<IDiscovery>();
            var error = new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found");
            mockDiscovery.Setup(x => x.PatientFor(discoveryRequest)).ReturnsAsync(
                new Tuple<hip_library.Patient.models.Patient, Error>(null,
                    error));

            var accountsController = new PatientController(mockDiscovery.Object);
            var response = accountsController.Discover(discoveryRequest).Result as NotFoundObjectResult;

            mockDiscovery.Verify();
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}