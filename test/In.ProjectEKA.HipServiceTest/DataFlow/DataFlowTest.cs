namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using Bogus.DataSets;
    using FluentAssertions;
    using In.ProjectEKA.HipService.DataFlow;
    using Builder;
    using HipLibrary.Patient.Model;
    using Moq;
    using Optional;
    using Xunit;
    using HipService.Consent;
    using HipService.MessagingQueue;
    using Microsoft.Extensions.Options;
    using DataFlowService = HipService.DataFlow.DataFlow;

    [Collection("Data Flow Tests")]
    public class DataFlowTest
    {
        private readonly Mock<IDataFlowRepository> dataFlowRepository = new Mock<IDataFlowRepository>();
        private readonly Mock<IConsentRepository> consentRepository = new Mock<IConsentRepository>();
        private readonly Mock<ILinkDataRepository> linkDataRepository = new Mock<ILinkDataRepository>();
        private readonly Mock<IMessagingQueueManager> messagingQueueManager = new Mock<IMessagingQueueManager>();
        private readonly IOptions<DataFlowConfiguration> dataFlowConfiguration;
        private readonly DataFlowService dataFlowService;

        public DataFlowTest()
        {
            var configuration = new DataFlowConfiguration {DataSizeLimitInMbs = 5, DataLinkTTLInMinutes = 5};
            dataFlowConfiguration = Options.Create(configuration);

            dataFlowService = new DataFlowService(
                dataFlowRepository.Object,
                messagingQueueManager.Object,
                consentRepository.Object,
                linkDataRepository.Object,
                dataFlowConfiguration);
        }

        [Fact]
        private async void ReturnTransactionIdOnSuccess()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            dataFlowRepository.Setup(d => d.SaveRequest(transactionId, request))
                .ReturnsAsync(Option.None<Exception>());
            consentRepository.Setup(d => d.GetFor(request.Consent.Id)).ReturnsAsync(TestBuilder.Consent());

            var (healthInformationResponse, _) = await dataFlowService.HealthInformationRequestFor(request);

            dataFlowRepository.Verify();
            healthInformationResponse.TransactionId.Should().BeEquivalentTo(transactionId);
        }

        [Fact]
        private async void ReturnErrorOnFailure()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            dataFlowRepository.Setup(d => d.SaveRequest(transactionId, request))
                .ReturnsAsync(Option.Some(new Exception()));
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.ServerInternalError,
                ErrorMessage.InternalServerError));
            consentRepository.Setup(d => d.GetFor(request.Consent.Id)).ReturnsAsync(TestBuilder.Consent());

            var (_, errorResponse) = await dataFlowService.HealthInformationRequestFor(request);

            dataFlowRepository.Verify();
            errorResponse.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetLinkData()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var linkId = TestBuilder.Faker().Random.Hash();
            var token = TestBuilder.Faker().Random.Hash();
            var linkData = TestBuilder.LinkData(token, DateTime.Now);

            linkDataRepository.Setup(x => x.GetAsync(linkId))
                .ReturnsAsync(linkData);

            var (healthInformation, errorRepresentation) =
                await dataFlowService.HealthInformationFor(linkId, token, transactionId);

            errorRepresentation.Should().BeNull();
            healthInformation.Should().BeEquivalentTo(new LinkDataResponse(transactionId, linkData.Data));
        }

        [Fact]
        private async void ShouldGetLinkDataNotFoundOnGetHealthInformation()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var linkId = TestBuilder.Faker().Random.Hash();

            var (_, errorRepresentation) = await dataFlowService.HealthInformationFor(linkId, "token", transactionId);

            var expectedError = new ErrorRepresentation(
                new Error(ErrorCode.LinkDataNotFound, ErrorMessage.LinkDataNotFound));
            errorRepresentation.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetInvalidTokenOnGetHealthInformation()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var linkId = TestBuilder.Faker().Random.Hash();
            var token = TestBuilder.Faker().Random.Hash();
            var linkData = TestBuilder.LinkData(token, TestBuilder.Faker().Date.Past());
            linkDataRepository.Setup(x => x.GetAsync(linkId))
                .ReturnsAsync(linkData);

            var (_, errorRepresentation) = await dataFlowService
                .HealthInformationFor(linkId, "invalid-token", transactionId);

            var expectedError = new ErrorRepresentation(
                new Error(ErrorCode.InvalidToken, ErrorMessage.InvalidToken));
            errorRepresentation.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetLinkExpiredOnGetHealthInformation()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var linkId = TestBuilder.Faker().Random.Hash();
            var token = TestBuilder.Faker().Random.Hash();
            var linkData = TestBuilder.LinkData(token, TestBuilder.Faker().Date.Past());

            linkDataRepository.Setup(x => x.GetAsync(linkId))
                .ReturnsAsync(linkData);

            var (_, errorRepresentation) =
                await dataFlowService.HealthInformationFor(linkId, token, transactionId);
            var expectedError = new ErrorRepresentation(
                new Error(ErrorCode.LinkExpired, ErrorMessage.LinkExpired));
            errorRepresentation.Should().BeEquivalentTo(expectedError);
        }
    }
}