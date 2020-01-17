namespace In.ProjectEKA.HipServiceTest.Discovery.Patient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using HipLibrary.Patient.Model.Request;
    using HipLibrary.Patient.Model.Response;
    using HipService.Discovery.Patient;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;
    using Patient = HipLibrary.Patient.Model.Request.Patient;
    using static Builder.TestBuilders;

    [Collection("Patient Controller Tests")]
    public class PatientsControllerTest
    {
        private readonly PatientsController patientsController;

        private readonly Mock<IDiscovery> discovery;

        public PatientsControllerTest()
        {
            discovery = new Mock<IDiscovery>();
            patientsController = new PatientsController(discovery.Object);
        }

        [Fact]
        public async Task ShouldGetAPatient()
        {
            var verifiedIdentifiers = identifier()
                .GenerateLazy(1)
                .Select(builder => builder.build());
            var unverifiedIdentifiers = identifier()
                .GenerateLazy(1)
                .Select(builder => builder.build());
            var discoveryRequest = new DiscoveryRequest(
                new Patient(Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    Faker().Name.FirstName(),
                    Faker().Name.FirstName(),
                    Faker().PickRandom<Gender>(),
                    Faker().Date.Past()), Faker().Random.String());
            var expectedPatient = new HipLibrary.Patient.Model.Response.Patient("p1", "J K",
                new List<CareContextRepresentation>
                {
                    new CareContextRepresentation("1", "display")
                }, new List<string>());
            var expectedResponse = new DiscoveryResponse(expectedPatient);
            discovery.Setup(x => x.PatientFor(discoveryRequest)).ReturnsAsync(
                new Tuple<DiscoveryResponse, ErrorResponse>(expectedResponse, null));

            var response = await patientsController.Discover(discoveryRequest);

            discovery.Verify();
            response.Should()
                .NotBeNull()
                .And
                .Subject.As<OkObjectResult>()
                .Value
                .Should()
                .BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task ShouldGetNotFound()
        {
            var verifiedIdentifiers = identifier()
                .GenerateLazy(1)
                .Select(builder => builder.build());
            var unverifiedIdentifiers = identifier()
                .GenerateLazy(1)
                .Select(builder => builder.build());
            var discoveryRequest = new DiscoveryRequest(
                new Patient(Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    Faker().Name.FirstName(),
                    Faker().Name.FirstName(),
                    Faker().PickRandom<Gender>(),
                    Faker().Date.Past()), Faker().Random.String());
            var error = new ErrorResponse(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
            discovery.Setup(x => x.PatientFor(discoveryRequest)).ReturnsAsync(
                new Tuple<DiscoveryResponse, ErrorResponse>(null, error));

            var response = await patientsController.Discover(discoveryRequest);

            discovery.Verify();
            response.Should().NotBeNull()
                .And
                .BeOfType<NotFoundObjectResult>()
                .Subject.StatusCode.Should()
                .Be(StatusCodes.Status404NotFound);
        }
    }
}