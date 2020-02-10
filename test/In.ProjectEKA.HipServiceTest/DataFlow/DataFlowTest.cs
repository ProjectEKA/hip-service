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
    using System.Collections.Generic;
    using Bogus;
    using HipService.MessagingQueue;
    
    using DataFlowService = HipService.DataFlow.DataFlow;
    
    [Collection("Data Flow Tests")]
    public class DataFlowTest
    {
        private readonly Mock<IDataFlowRepository> dataFlowRepository = new Mock<IDataFlowRepository>();
        private readonly Mock<IMessagingQueueManager> messagingQueueManager = new Mock<IMessagingQueueManager>();
        private readonly Mock<IConsentArtefactRepository> consentArtefactRepository = new Mock<IConsentArtefactRepository>();
        private readonly DataFlowService dataFlowService;

        public DataFlowTest()
        {
            dataFlowService = new DataFlowService(
                dataFlowRepository.Object,
                consentArtefactRepository.Object,
                messagingQueueManager.Object);
        }

        [Fact]
        private async void ReturnTransactionIdOnSuccess()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            dataFlowRepository.Setup(d => d.SaveRequestFor(transactionId, request))
                .ReturnsAsync(Option.None<Exception>());
            consentArtefactRepository.Setup(d => d.GetFor(request.Consent.Id)).
                Returns(new Tuple<ConsentArtefact, Exception>(TestBuilder.ConsentArtefact().Generate(), null));

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
            consentArtefactRepository.Setup(d => d.GetFor(request.Consent.Id)).
                Returns(new Tuple<ConsentArtefact, Exception>(TestBuilder.ConsentArtefact().Generate(), null));
            
            var (_, errorResponse) = await dataFlowService.HealthInformationRequestFor(request);
            
            dataFlowRepository.Verify();
            errorResponse.Should().BeEquivalentTo(expectedError);
        }
    }
}