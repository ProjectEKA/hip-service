namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using Builder;
    using FluentAssertions;
    using Hangfire;
    using Hangfire.Common;
    using Hangfire.States;
    using HipLibrary.Patient.Model;
    using HipService.DataFlow;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using Xunit;
    using HipService.Gateway;
    

    public class PatientDataFlowControllerTest
    {
        private readonly Mock<IBackgroundJobClient> backgroundJobClient = new Mock<IBackgroundJobClient>();
        private readonly Mock<IDataFlow> dataFlow = new Mock<IDataFlow>();
        private readonly PatientDataFlowController patientDataFlowController;

        private readonly Mock<GatewayClient> gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null, null);

        public PatientDataFlowControllerTest()
        {
            patientDataFlowController = new PatientDataFlowController(dataFlow.Object, backgroundJobClient.Object,gatewayClient.Object);
        }

        [Fact]
        private void ShouldEnqueueDataFlowRequestAndReturnAccepted()
        {
            var gatewayId = TestBuilder.Faker().Random.String();
            var transactionId = TestBuilder.Faker().Random.Hash();
            var requestId = TestBuilder.Faker().Random.Hash();

            var healthInformationRequest = TestBuilder.HealthInformationRequest(transactionId);
            var hiRequest = new HIRequest(healthInformationRequest.Consent,
                                          healthInformationRequest.DateRange,
                                          healthInformationRequest.DataPushUrl,
                                          healthInformationRequest.KeyMaterial);

            var request = new PatientHealthInformationRequest(transactionId, requestId, It.IsAny<DateTime>(), hiRequest);
            var expectedResponse = new HealthInformationTransactionResponse(transactionId);
            dataFlow.Setup(d => d.HealthInformationRequestFor(healthInformationRequest, gatewayId))
                .ReturnsAsync(
                    new Tuple<HealthInformationTransactionResponse, ErrorRepresentation>(expectedResponse, null));

            var response = patientDataFlowController.HealthInformationRequestFor(request, gatewayId);

            backgroundJobClient.Verify(client => client.Create(
                It.Is<Job>(job => job.Method.Name == "HealthInformationOf" && job.Args[0] == request),
                It.IsAny<EnqueuedState>()
            ));

            dataFlow.Verify();
            response.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }
    }
}