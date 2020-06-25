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
    using HipService.Gateway.Model;

    public class PatientDataFlowControllerTest
    {
        private readonly Mock<IBackgroundJobClient> backgroundJobClient = new Mock<IBackgroundJobClient>();
        private readonly Mock<IDataFlow> dataFlow = new Mock<IDataFlow>();
        private readonly PatientDataFlowController patientDataFlowController;

        private readonly Mock<GatewayClient> gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null);

        public PatientDataFlowControllerTest()
        {
            patientDataFlowController = new PatientDataFlowController(dataFlow.Object,
                backgroundJobClient.Object,
                gatewayClient.Object);
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
                It.IsAny<EnqueuedState>()));
            dataFlow.Verify();
            response.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }

        [Fact]
        private async void ShouldSendDataFlowRequestToGateway()
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
            gatewayClient.Setup(
                client =>
                    client.SendDataToGateway("/v1/health-information/hip/on-request",
                        It.IsAny<GatewayDataFlowRequestResponse>(), "ncg"));
            dataFlow.Setup(d => d.GetPatientId(It.IsAny<string>())).ReturnsAsync("abc@ncg");
            await patientDataFlowController.HealthInformationOf(request, gatewayId);

            gatewayClient.Verify();
            dataFlow.Verify();
        }
    }
}