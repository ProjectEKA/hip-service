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
    using HipService.MessagingQueue;
    
    using DataFlowService = HipService.DataFlow.DataFlow;
    
    [Collection("Data Flow Tests")]
    public class DataFlowTest
    {
        private readonly Mock<IDataFlowRepository> dataFlowRepository = new Mock<IDataFlowRepository>();
        private readonly Mock<IMessagingQueueManager> messagingQueueManager = new Mock<IMessagingQueueManager>();
        private readonly Mock<IDataFlowArtefactRepository> dataFlowArtefactRepository = new Mock<IDataFlowArtefactRepository>();
        private readonly DataFlowService dataFlowService;

        public DataFlowTest()
        {
            dataFlowService = new DataFlowService(
                dataFlowRepository.Object,
                dataFlowArtefactRepository.Object,
                messagingQueueManager.Object);
        }

        [Fact]
        private async void ReturnTransactionIdOnSuccess()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            var dataFlowArtefact = new DataFlowArtefact(
                new List<GrantedContext> {new GrantedContext("5",
                    "130")},
                request.HiDataRange,
                request.CallBackUrl,
                new List<HiType> {HiType.Condition});
            dataFlowRepository.Setup(d => d.SaveRequestFor(transactionId, request))
                .ReturnsAsync(Option.None<Exception>());
            dataFlowArtefactRepository.Setup(d => d.GetFor(request)).
                Returns(new Tuple<DataFlowArtefact, ErrorRepresentation>(dataFlowArtefact, null));

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
            dataFlowArtefactRepository.Setup(d => d.GetFor(request)).
                Returns(new Tuple<DataFlowArtefact, ErrorRepresentation>(null, expectedError));
            
            var (_, errorResponse) = await dataFlowService.HealthInformationRequestFor(request);
            
            dataFlowRepository.Verify();
            errorResponse.Should().BeEquivalentTo(expectedError);
        }
    }
}