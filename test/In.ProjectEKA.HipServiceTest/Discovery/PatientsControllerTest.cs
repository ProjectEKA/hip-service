namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;
    using static Builder.TestBuilders;

    [Collection("Patient Controller Tests")]
    public class PatientsControllerTest
    {
        private readonly DiscoveryController discoveryController;

        private readonly Mock<PatientDiscovery> discovery;

        public PatientsControllerTest()
        {
            discovery = new Mock<PatientDiscovery>(MockBehavior.Strict, null, null, null, null);
            discoveryController = new DiscoveryController(discovery.Object);
        }

        [Fact]
        public async Task ShouldGetAPatient()
        {
            var verifiedIdentifiers = Identifier()
                .GenerateLazy(1)
                .Select(builder => builder.Build());
            var unverifiedIdentifiers = Identifier()
                .GenerateLazy(1)
                .Select(builder => builder.Build());
            var discoveryRequest = new DiscoveryRequest(
                new PatientEnquiry(Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    Faker().Name.FullName(),
                    Faker().PickRandom<Gender>(),
                    (ushort) Faker().Date.Past().Year), Faker().Random.String());
            var expectedPatient = new PatientEnquiryRepresentation(
                "p1",
                "J K",
                new List<CareContextRepresentation>
                {
                    new CareContextRepresentation("1", "display")
                },
                new List<string>());
            var expectedResponse = new DiscoveryRepresentation(expectedPatient);
            discovery.Setup(x => x.PatientFor(discoveryRequest)).ReturnsAsync((expectedResponse, null));

            var response = await discoveryController.Discover(discoveryRequest);

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
            var verifiedIdentifiers = Identifier()
                .GenerateLazy(1)
                .Select(builder => builder.Build());
            var unverifiedIdentifiers = Identifier()
                .GenerateLazy(1)
                .Select(builder => builder.Build());
            var discoveryRequest = new DiscoveryRequest(
                new PatientEnquiry(Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    Faker().Name.FullName(),
                    Faker().PickRandom<Gender>(),
                    (ushort) Faker().Date.Past().Year), Faker().Random.String());
            var error = new ErrorRepresentation(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
            discovery.Setup(x => x.PatientFor(discoveryRequest)).ReturnsAsync((null, error));

            var response = await discoveryController.Discover(discoveryRequest);

            discovery.Verify();
            response.Should().NotBeNull()
                .And
                .BeOfType<NotFoundObjectResult>()
                .Subject.StatusCode.Should()
                .Be(StatusCodes.Status404NotFound);
        }
    }
}