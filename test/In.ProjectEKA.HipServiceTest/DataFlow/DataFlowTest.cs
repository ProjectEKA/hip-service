namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using FluentAssertions;
    using HipLibrary.Patient.Model.Response;
    using In.ProjectEKA.HipService.DataFlow;
    using Builder;
    using Moq;
    using Optional;
    using Xunit;
    
    using DataFlowService = HipService.DataFlow.DataFlow;
    
    [Collection("Data Flow Tests")]
    public class DataFlowTest
    {
        private readonly Mock<IDataFlowRepository> dataFlowRepository = new Mock<IDataFlowRepository>();
        private readonly DataFlowService dataFlowService;

        public DataFlowTest()
        {
            dataFlowService = new DataFlowService(dataFlowRepository.Object);
        }

        [Fact]
        private async void ReturnTransactionIdOnSuccess()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            dataFlowRepository.Setup(d => d.SaveRequestFor(transactionId, request))
                .ReturnsAsync(Option.None<Exception>());

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
            var expectedError = new ErrorResponse(new Error(ErrorCode.ServerInternalError,
                ErrorMessage.InternalServerError));
            
            var (_, errorResponse) = await dataFlowService.HealthInformationRequestFor(request);
            
            dataFlowRepository.Verify();
            errorResponse.Should().BeEquivalentTo(expectedError);
        }
    }
}