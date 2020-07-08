namespace In.ProjectEKA.HipServiceTest.Link
{
    using FluentAssertions;
    using Hangfire;
    using Hangfire.Common;
    using Hangfire.States;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using HipService.Gateway;
    using HipService.Link;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using Xunit;
    using static Builder.TestBuilders;

    [Collection("Link Controller Tests")]
    public class LinkControllerTest
    {
        private readonly Mock<LinkPatient> link;
        private readonly Mock<IDiscoveryRequestRepository> discoveryRequestRepository;
        private readonly LinkController linkController;
        private readonly Mock<IBackgroundJobClient> backgroundJobClient;

        public LinkControllerTest()
        {
            link = new Mock<LinkPatient>(MockBehavior.Strict, null, null, null, null, null, null);
            discoveryRequestRepository = new Mock<IDiscoveryRequestRepository>();
            var gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null);

            backgroundJobClient = new Mock<IBackgroundJobClient>();
            linkController = new LinkController(
                discoveryRequestRepository.Object,
                backgroundJobClient.Object,
                link.Object,
                gatewayClient.Object);
        }

        [Fact]
        private void ShouldEnqueueLinkRequestAndReturnAccepted()
        {
            var faker = Faker();
            const string programRefNo = "129";
            const string patientReference = "4";
            var id = faker.Random.Hash();
            var transactionId = faker.Random.Hash();
            var linkRequest = new LinkReferenceRequest(
                transactionId,
                new PatientLinkReference(
                    id,
                    patientReference,
                    new[] {new CareContextEnquiry(programRefNo)}),
                faker.Random.Hash());

            discoveryRequestRepository.Setup(x => x.RequestExistsFor(linkRequest.TransactionId,
                id,
                linkRequest.Patient.ReferenceNumber))
                .ReturnsAsync(true);

            var linkedResult = linkController.LinkFor(linkRequest);

            backgroundJobClient.Verify(client => client.Create(
                It.Is<Job>(job => job.Method.Name == "LinkPatient" && job.Args[0] == linkRequest),
                It.IsAny<EnqueuedState>()
                ));

            link.Verify();
            discoveryRequestRepository.Verify();
            linkedResult.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }

        [Fact]
        private void ShouldEnqueueLinkConfirmationRequestAndReturnAccepted()
        {
            var linkReferenceNumber = Faker().Random.Hash();
            const string token = "1234";
            var cmId = "ncg";
            var careContext = new[] {new CareContextRepresentation("129", Faker().Random.Word())};
            var expectedResponse = new PatientLinkConfirmationRepresentation(new LinkConfirmationRepresentation("4",
                Faker().Random.Word()
                , careContext));
            link.Setup(e => e.VerifyAndLinkCareContext(It.Is<LinkConfirmationRequest>(p =>
                p.Token == "1234" &&
                p.LinkReferenceNumber == linkReferenceNumber)))
                .ReturnsAsync((expectedResponse,cmId,null));

            var linkPatientRequest = new LinkPatientRequest(Faker().Random.Hash(),
                It.IsAny<string>(),
                new LinkConfirmation(linkReferenceNumber, token));
            var response = linkController.LinkPatientFor(linkPatientRequest);

            backgroundJobClient.Verify(client => client.Create(
                It.Is<Job>(job => job.Method.Name == "LinkPatientCareContextFor" && job.Args[0] == linkPatientRequest),
                It.IsAny<EnqueuedState>()));

            link.Verify();
            discoveryRequestRepository.Verify();
            response.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }
    }
}