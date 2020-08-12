namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using FluentAssertions;
    using In.ProjectEKA.HipService.DataFlow;
    using Builder;
    using HipLibrary.Patient.Model;
    using Moq;
    using Optional;
    using Xunit;
    using HipService.Consent;
    using HipService.MessagingQueue;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using DataFlowService = HipService.DataFlow.DataFlow;

    [Collection("Data Flow Tests")]
    public class DataFlowTest
    {
        private readonly Mock<IDataFlowRepository> dataFlowRepository = new Mock<IDataFlowRepository>();
        private readonly Mock<IConsentRepository> consentRepository = new Mock<IConsentRepository>();
        private readonly Mock<ILogger<DataFlow>> loggerMock = new Mock<ILogger<DataFlow>>();

        private readonly Mock<IHealthInformationRepository> healthInformationRepository =
            new Mock<IHealthInformationRepository>();

        private readonly Mock<IMessagingQueueManager> messagingQueueManager = new Mock<IMessagingQueueManager>();
        private readonly DataFlowService dataFlowService;

        public DataFlowTest()
        {
            var configuration = new DataFlowConfiguration {DataSizeLimitInMbs = 5, DataLinkTtlInMinutes = 5};
            var dataFlowConfiguration = Options.Create(configuration);

            dataFlowService = new DataFlowService(
                dataFlowRepository.Object,
                messagingQueueManager.Object,
                consentRepository.Object,
                healthInformationRepository.Object,
                dataFlowConfiguration,
                loggerMock.Object);
        }

        [Fact]
        private async void ReturnAcknowledgementIdOnSuccess()
        {
            var consentMangerId = TestBuilder.Faker().Random.String();
            var transactionId = TestBuilder.Faker().Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            dataFlowRepository.Setup(d => d.SaveRequest(transactionId, request))
                .ReturnsAsync(Option.None<Exception>());
            consentRepository.Setup(d => d.GetFor(request.Consent.Id))
                .ReturnsAsync(TestBuilder.DataFlowConsent());

            var (healthInformationResponse, _) =
                await dataFlowService.HealthInformationRequestFor(request, consentMangerId);

            dataFlowRepository.Verify();
            healthInformationResponse.AcknowledgementId.Should().BeEquivalentTo(transactionId);
        }

        [Fact]
        private async void ReturnErrorOnFailure()
        {
            var consentMangerId = TestBuilder.Faker().Random.String();

            var transactionId = TestBuilder.Faker().Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            dataFlowRepository.Setup(d => d.SaveRequest(transactionId, request))
                .ReturnsAsync(Option.Some(new Exception()));
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.ServerInternalError,
                ErrorMessage.InternalServerError));
            consentRepository.Setup(d => d.GetFor(request.Consent.Id))
                .ReturnsAsync(TestBuilder.DataFlowConsent());

            var (_, errorResponse) = await dataFlowService.HealthInformationRequestFor(request, consentMangerId);

            dataFlowRepository.Verify();
            errorResponse.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetHealthInformation()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var linkId = TestBuilder.Faker().Random.Hash();
            var token = TestBuilder.Faker().Random.Hash();
            var healthInformation = TestBuilder.HealthInformation(token, DateTime.Now);

            healthInformationRepository.Setup(x => x.GetAsync(linkId))
                .ReturnsAsync(Option.Some(healthInformation));

            var (healthInformationResponse, errorRepresentation) =
                await dataFlowService.HealthInformationFor(linkId, token);

            errorRepresentation.Should().BeNull();
            healthInformationResponse.Should()
                .BeEquivalentTo(new HealthInformationResponse(healthInformation.Data.Content));
        }

        [Fact]
        private async void ShouldGetHealthInformationNotFound()
        {
            var linkId = TestBuilder.Faker().Random.Hash();

            var (_, errorRepresentation) = await dataFlowService.HealthInformationFor(linkId, "token");

            var expectedError = new ErrorRepresentation(
                new Error(ErrorCode.HealthInformationNotFound, ErrorMessage.HealthInformationNotFound));
            errorRepresentation.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetInvalidTokenOnGetHealthInformation()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var linkId = TestBuilder.Faker().Random.Hash();
            var token = TestBuilder.Faker().Random.Hash();
            var healthInformation = TestBuilder.HealthInformation(token, TestBuilder.Faker().Date.Past());
            healthInformationRepository.Setup(x => x.GetAsync(linkId))
                .ReturnsAsync(Option.Some(healthInformation));

            var (_, errorRepresentation) = await dataFlowService
                .HealthInformationFor(linkId, "invalid-token");

            var expectedError = new ErrorRepresentation(
                new Error(ErrorCode.InvalidToken, ErrorMessage.InvalidToken));
            errorRepresentation.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetLinkExpiredOnGetHealthInformation()
        {
            var linkId = TestBuilder.Faker().Random.Hash();
            var token = TestBuilder.Faker().Random.Hash();
            var healthInformation = TestBuilder.HealthInformation(token, TestBuilder.Faker().Date.Past());

            healthInformationRepository.Setup(x => x.GetAsync(linkId))
                .ReturnsAsync(Option.Some(healthInformation));

            var (_, errorRepresentation) =
                await dataFlowService.HealthInformationFor(linkId, token);
            var expectedError = new ErrorRepresentation(
                new Error(ErrorCode.LinkExpired, ErrorMessage.LinkExpired));
            errorRepresentation.Should().BeEquivalentTo(expectedError);
        }
    }
}