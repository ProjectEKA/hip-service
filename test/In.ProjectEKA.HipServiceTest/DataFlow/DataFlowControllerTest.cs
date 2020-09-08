using Hl7.Fhir.Model;

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
            var consentMangerId = TestBuilder.Faker().Random.String();
            var transactionId = TestBuilder.Faker().Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            var expectedResponse = new HealthInformationTransactionResponse(transactionId);
            var correlationId = Uuid.Generate().ToString();
            dataFlow.Setup(d => d.HealthInformationRequestFor(request, consentMangerId, correlationId))
                .ReturnsAsync(
                    new Tuple<HealthInformationTransactionResponse, ErrorRepresentation>(expectedResponse, null));

            var response = await dataFlowController.HealthInformationRequestFor(request, correlationId,  consentMangerId);

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
            var consentMangerId = TestBuilder.Faker().Random.String();
            var request = TestBuilder.HealthInformationRequest(TestBuilder.Faker().Random.Hash());
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.ServerInternalError,
                ErrorMessage.InternalServerError));
            var correlationId = Uuid.Generate().ToString();
            dataFlow.Setup(d => d.HealthInformationRequestFor(request, consentMangerId, correlationId))
                .ReturnsAsync(
                    new Tuple<HealthInformationTransactionResponse, ErrorRepresentation>(null, expectedError));

            var response =
                await dataFlowController.HealthInformationRequestFor(request, correlationId,  consentMangerId) as ObjectResult;

            dataFlow.Verify();
            response.StatusCode
                .Should()
                .Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        private async void ShouldGetHealthInformation()
        {
            var linkId = TestBuilder.Faker().Random.Hash();
            var transactionId = TestBuilder.Faker().Random.Hash();
            var healthInformationResponse = TestBuilder.HealthInformationResponse(transactionId);
            var token = TestBuilder.Faker().Random.String();

            dataFlow.Setup(x => x.HealthInformationFor(linkId, token))
                .ReturnsAsync(
                    new Tuple<HealthInformationResponse, ErrorRepresentation>(healthInformationResponse, null));

            var healthInformation = await dataFlowController
                .HealthInformation(linkId, token) as OkObjectResult;

            healthInformation.StatusCode.Should().Be(StatusCodes.Status200OK);
            healthInformation.Value.Should().BeEquivalentTo(healthInformationResponse);
        }

        [Fact]
        private async void ShouldGetHealthInformationNotFound()
        {
            var linkId = TestBuilder.Faker().Random.Hash();
            var transactionId = TestBuilder.Faker().Random.Hash();
            var expectedError = new ErrorRepresentation(
                new Error(ErrorCode.HealthInformationNotFound, ErrorMessage.HealthInformationNotFound));
            var token = TestBuilder.Faker().Random.String();

            dataFlow.Setup(x => x.HealthInformationFor(linkId, token))
                .ReturnsAsync(new Tuple<HealthInformationResponse, ErrorRepresentation>(null, expectedError));

            var healthInformation = await dataFlowController
                .HealthInformation(linkId, token) as ObjectResult;

            healthInformation.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            healthInformation.Value.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetInvalidTokenOnGetHealthInformation()
        {
            var linkId = TestBuilder.Faker().Random.Hash();
            var transactionId = TestBuilder.Faker().Random.Hash();
            var expectedError = new ErrorRepresentation(
                new Error(ErrorCode.InvalidToken, ErrorMessage.InvalidToken));
            var token = TestBuilder.Faker().Random.String();

            dataFlow.Setup(x => x.HealthInformationFor(linkId, token))
                .ReturnsAsync(new Tuple<HealthInformationResponse, ErrorRepresentation>(null, expectedError));

            var healthInformation = await dataFlowController
                .HealthInformation(linkId, token) as ObjectResult;

            healthInformation.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            healthInformation.Value.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetLinkExpiredOnGetHealthInformation()
        {
            var linkId = TestBuilder.Faker().Random.Hash();
            var transactionId = TestBuilder.Faker().Random.Hash();
            var expectedError = new ErrorRepresentation(
                new Error(ErrorCode.LinkExpired, ErrorMessage.LinkExpired));
            const string token = "token";

            dataFlow.Setup(x => x.HealthInformationFor(linkId, token))
                .ReturnsAsync(new Tuple<HealthInformationResponse, ErrorRepresentation>(null, expectedError));

            var healthInformation = await dataFlowController
                .HealthInformation(linkId, token) as ObjectResult;

            healthInformation.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            healthInformation.Value.Should().BeEquivalentTo(expectedError);
        }
    }
}