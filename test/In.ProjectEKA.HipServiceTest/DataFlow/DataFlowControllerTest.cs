namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using FluentAssertions;
    using In.ProjectEKA.HipService.DataFlow;
    using Builder;
    using HipLibrary.Patient.Model;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;
    using Microsoft.AspNetCore.Http;

    [Collection("Data Flow Controller Tests")]
    public class DataFlowControllerTest
    {
        private readonly DataFlowController dataFlowController;
        private readonly Mock<IDataFlow> dataFlow = new Mock<IDataFlow>();

        public DataFlowControllerTest()
        {
            dataFlowController = new DataFlowController(dataFlow.Object);
        }

        [Fact]
        private async void ReturnTransactionId()
        {
            var transactionId = TestBuilder.Faker().Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            var expectedResponse = new HealthInformationResponse(transactionId);
            dataFlow.Setup(d => d.HealthInformationRequestFor(request))
                .ReturnsAsync(new Tuple<HealthInformationResponse, ErrorRepresentation>(expectedResponse, null));

            var response = await dataFlowController.HealthInformationRequestFor(request);
            
            dataFlow.Verify();
            response.Should()
                .NotBeNull()
                .And
                .Subject.As<OkObjectResult>()
                .Value
                .Should()
                .BeEquivalentTo(expectedResponse);
        }

        [Fact]
        private async void CheckInternalServerErrorOnSaveDataFailure()
        {
            var request = TestBuilder.HealthInformationRequest(TestBuilder.Faker().Random.Hash());
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.ServerInternalError,
                ErrorMessage.InternalServerError));
            dataFlow.Setup(d => d.HealthInformationRequestFor(request))
                .ReturnsAsync(new Tuple<HealthInformationResponse, ErrorRepresentation>(null, expectedError));
            
            var response = await dataFlowController.HealthInformationRequestFor(request) as ObjectResult;
            
            dataFlow.Verify();
            response.StatusCode
                .Should()
                .Be(StatusCodes.Status500InternalServerError);
        }
    }
}