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
    
    using DataFlowService = HipService.DataFlow.DataFlow;
    
    [Collection("Data Flow Tests")]
    public class DataFlowTest
    {
        private readonly Mock<IDataFlowRepository> dataFlowRepository = new Mock<IDataFlowRepository>();
        private readonly Mock<IConsentRepository> consentRepository = new Mock<IConsentRepository>();
        private readonly Mock<IMessagingQueueManager> messagingQueueManager = new Mock<IMessagingQueueManager>();
        private readonly DataFlowService dataFlowService;

        public DataFlowTest()
        {
            dataFlowService = new DataFlowService(
                dataFlowRepository.Object,
                messagingQueueManager.Object,
                consentRepository.Object);
        }

        [Fact]
        private async void ReturnTransactionIdOnSuccess()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            dataFlowRepository.Setup(d => d.SaveRequestFor(transactionId, request))
                .ReturnsAsync(Option.None<Exception>());
            consentRepository.Setup(d => d.GetFor(request.Consent.Id)).
                ReturnsAsync(TestBuilder.Consent());

            var (healthInformationResponse, _) = await dataFlowService.HealthInformationRequestFor(request);

            dataFlowRepository.Verify();
            healthInformationResponse.TransactionId.Should().BeEquivalentTo(transactionId);
        }

        [Fact]
        private async void ReturnErrorOnFailure()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            dataFlowRepository.Setup(d => d.SaveRequestFor(transactionId, request))
                .ReturnsAsync(Option.Some(new Exception()));
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.ServerInternalError,
                ErrorMessage.InternalServerError));
            consentRepository.Setup(d => d.GetFor(request.Consent.Id)).
                ReturnsAsync(TestBuilder.Consent());
            
            var (_, errorResponse) = await dataFlowService.HealthInformationRequestFor(request);
            
            dataFlowRepository.Verify();
            errorResponse.Should().BeEquivalentTo(expectedError);
        }
    }
}