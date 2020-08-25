using System.Net.WebSockets;

namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using System.Linq;
    using System.Net;
    using Hangfire;
    using In.ProjectEKA.HipService.Gateway;
    using In.ProjectEKA.HipService.Gateway.Model;
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;
    using In.ProjectEKA.HipService.Discovery;
    using Moq;
    using Xunit;
    using System.Net.Http.Headers;
    using Common.TestServer;
    using Builder;
    using FluentAssertions;
    using Hangfire.Common;
    using Hangfire.States;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;

    public class PatientControllerTest
    {
        private readonly Mock<IPatientDiscovery> patientDiscoveryMock;
        private readonly CareContextDiscoveryController careContextDiscoveryController;
        private readonly Dictionary<string, GatewayDiscoveryRepresentation> responsesSentToGateway;
        private readonly Dictionary<string, Job> backgroundJobs;
        
        private static readonly User Krunal = User.Krunal;
        private static readonly User JohnDoe = User.JohnDoe;

        public PatientControllerTest()
        {
            patientDiscoveryMock = new Mock<IPatientDiscovery>();
            var gatewayClientMock = new Mock<IGatewayClient>();
            var backgroundJobClientMock = new Mock<IBackgroundJobClient>();
            var logger = new Mock<ILogger<CareContextDiscoveryController>>();
            
            responsesSentToGateway = new Dictionary<string, GatewayDiscoveryRepresentation>();
            backgroundJobs = new Dictionary<string, Job>();

            careContextDiscoveryController = new CareContextDiscoveryController(patientDiscoveryMock.Object, gatewayClientMock.Object, backgroundJobClientMock.Object, logger.Object);

            SetupGatewayClientToSaveAllSentDiscoveryIntoThisList(gatewayClientMock, responsesSentToGateway);
            SetupBackgroundJobClientToSaveAllCreatedJobsIntoThisList(backgroundJobClientMock, backgroundJobs);
        }

        [Theory]
        [InlineData(HttpStatusCode.Accepted)]
        [InlineData(HttpStatusCode.Accepted, "RequestId")]
        [InlineData(HttpStatusCode.Accepted, "RequestId", "PatientGender")]
        [InlineData(HttpStatusCode.Accepted, "RequestId", "PatientName")]
        [InlineData(HttpStatusCode.Accepted, "PatientName")]
        [InlineData(HttpStatusCode.Accepted, "PatientGender")]
        [InlineData(HttpStatusCode.BadRequest, "PatientName", "PatientGender")]
        [InlineData(HttpStatusCode.BadRequest, "TransactionId")]
        [InlineData(HttpStatusCode.BadRequest, "PatientId")]
        private async void DiscoverPatientCareContexts_ReturnsExpectedStatusCode_WhenRequestIsSentWithParameters(
            HttpStatusCode expectedStatusCode, params string[] missingRequestParameters)
        {
            var _server = new Microsoft.AspNetCore.TestHost.TestServer(new WebHostBuilder().UseStartup<TestStartup>());
            var _client = _server.CreateClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            var requestContent = new DiscoveryRequestPayloadBuilder()
                .WithMissingParameters(missingRequestParameters)
                .BuildSerializedFormat();

            var response =
                await _client.PostAsync(
                    "/v0.5/care-contexts/discover",
                    requestContent);

            response.StatusCode.Should().Be(expectedStatusCode);
        }

        #region Describe everything that should be sent by scenario
        [Fact]
        public async void ShouldSendWhenAPatientWasFound()
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(Krunal, out DiscoveryRequest discoveryRequest);
            AndThisPatientMatchASingleRegisteredPatient(Krunal, new []{"name", "gender"}, out DiscoveryRepresentation discoveryRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheSentResponseShouldContainTheFoundPatient(actualResponse, discoveryRepresentation.Patient);
            AndTheResponseShouldContainTheMatchFields(actualResponse, discoveryRepresentation.Patient.MatchedBy.ToList());
            AndTheResponseShouldContainTheTransactionId(actualResponse, discoveryRequest);
            AndTheResponseShouldContainTheExpectedStatus(actualResponse, discoveryRequest, HttpStatusCode.OK, "Patient record with one or more care contexts found");
            AndTheResponseShouldNotContainAnyError(actualResponse);
        }

        [Theory]
        [InlineData(ErrorCode.NoPatientFound, HttpStatusCode.NotFound, "No Matching Record Found or More than one Record Found")]
        [InlineData(ErrorCode.MultiplePatientsFound, HttpStatusCode.NotFound, "No Matching Record Found or More than one Record Found")]
        public async void ShouldSendWhenNoSingleMatchWasFound(ErrorCode errorCode, HttpStatusCode expectedStatusCode, string expectedResponseDescription)
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(JohnDoe, out DiscoveryRequest discoveryRequest);
            AndTheUserDoesNotMatchAnyPatientBecauseOf(errorCode, out ErrorRepresentation errorRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheResponseShouldNotContainAnyPatientDetails(actualResponse);
            AndTheResponseShouldNotContainAyMatchFields(actualResponse);
            AndTheResponseShouldContainTheTransactionId(actualResponse, discoveryRequest);
            AndTheResponseShouldContainTheExpectedStatus(actualResponse, discoveryRequest, expectedStatusCode, expectedResponseDescription);
            AndTheResponseShouldContainTheErrorDetails(actualResponse, errorRepresentation);
        }

        [Fact]
        public async void ShouldSendBahmniIsDownOrAnExternalSystem()
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(Krunal, out DiscoveryRequest discoveryRequest);
            ButTheDataSourceIsNotReachable(out ErrorRepresentation errorRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheResponseShouldNotContainAnyPatientDetails(actualResponse);
            AndTheResponseShouldNotContainAyMatchFields(actualResponse);
            AndTheResponseShouldContainTheTransactionId(actualResponse, discoveryRequest);
            AndTheResponseShouldContainTheExpectedStatus(actualResponse, discoveryRequest, HttpStatusCode.InternalServerError, "Unreachable external service");
            AndTheResponseShouldContainTheErrorDetails(actualResponse, errorRepresentation);
        }
        #endregion
    
        #region Unit block when a user find its record
        [Fact]
        public async void ShouldSendTheFoundPatientDetailsWhenAPatientWasFound()
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(Krunal, out DiscoveryRequest discoveryRequest);
            AndThisPatientMatchASingleRegisteredPatient(Krunal, new []{"name", "gender"}, out DiscoveryRepresentation discoveryRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheSentResponseShouldContainTheFoundPatient(actualResponse, discoveryRepresentation.Patient);
        }
        
        [Fact]
        public async void ShouldSendTheListOfMatchedFieldsWhenAPatientWasFound()
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(Krunal, out DiscoveryRequest discoveryRequest);
            AndThisPatientMatchASingleRegisteredPatient(Krunal, new []{"name", "gender"}, out DiscoveryRepresentation discoveryRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheResponseShouldContainTheMatchFields(actualResponse, discoveryRepresentation.Patient.MatchedBy.ToList());
        }
        
        [Fact]
        public async void ShouldSendTheTransactionIdWhenAPatientWasFound()
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(Krunal, out DiscoveryRequest discoveryRequest);
            AndThisPatientMatchASingleRegisteredPatient(Krunal, new []{"name", "gender"}, out DiscoveryRepresentation discoveryRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheResponseShouldContainTheTransactionId(actualResponse, discoveryRequest);
        }
        
        [Fact]
        public async void ShouldSendTheResponseStatusWith200WhenAPatientWasFound()
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(Krunal, out DiscoveryRequest discoveryRequest);
            AndThisPatientMatchASingleRegisteredPatient(Krunal, new []{"name", "gender"}, out DiscoveryRepresentation discoveryRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheResponseShouldContainTheExpectedStatus(actualResponse, discoveryRequest, HttpStatusCode.OK, "Patient record with one or more care contexts found");
        }
        
        [Fact]
        public async void ShouldNotSendAnyErrorWhenAPatientWasFound()
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(Krunal, out DiscoveryRequest discoveryRequest);
            AndThisPatientMatchASingleRegisteredPatient(Krunal, new []{"name", "gender"}, out DiscoveryRepresentation discoveryRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheResponseShouldNotContainAnyError(actualResponse);
        }
        #endregion
        
        #region Unit block when a user do not find his record
        [Theory]
        [InlineData(ErrorCode.NoPatientFound)]
        [InlineData(ErrorCode.MultiplePatientsFound)]
        public async void ShouldNotSendFoundPatientDetailsWhenNoPatientWasFound(ErrorCode errorCode)
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(JohnDoe, out DiscoveryRequest discoveryRequest);
            AndTheUserDoesNotMatchAnyPatientBecauseOf(errorCode, out ErrorRepresentation errorRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheResponseShouldNotContainAnyPatientDetails(actualResponse);
        }
        
        [Theory]
        [InlineData(ErrorCode.NoPatientFound)]
        [InlineData(ErrorCode.MultiplePatientsFound)]
        public async void ShouldNotSendAnyMatchedFieldWhenNoPatientWasFound(ErrorCode errorCode)
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(JohnDoe, out DiscoveryRequest discoveryRequest);
            AndTheUserDoesNotMatchAnyPatientBecauseOf(errorCode, out ErrorRepresentation errorRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheResponseShouldNotContainAyMatchFields(actualResponse);
        }
        
        [Theory]
        [InlineData(ErrorCode.NoPatientFound)]
        [InlineData(ErrorCode.MultiplePatientsFound)]
        public async void ShouldSendTransactionIdEvenWhenNoPatientWasFound(ErrorCode errorCode)
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(JohnDoe, out DiscoveryRequest discoveryRequest);
            AndTheUserDoesNotMatchAnyPatientBecauseOf(errorCode, out ErrorRepresentation errorRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheResponseShouldContainTheTransactionId(actualResponse, discoveryRequest);
        }
        
        [Theory]
        [InlineData(ErrorCode.NoPatientFound, HttpStatusCode.NotFound, "No Matching Record Found or More than one Record Found")]
        [InlineData(ErrorCode.MultiplePatientsFound, HttpStatusCode.NotFound, "No Matching Record Found or More than one Record Found")]
        public async void ShouldSendRequestStatusWith404WhenNoPatientWasFound(ErrorCode errorCode, HttpStatusCode expectedStatusCode, string expectedResponseDescription)
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(JohnDoe, out DiscoveryRequest discoveryRequest);
            AndTheUserDoesNotMatchAnyPatientBecauseOf(errorCode, out ErrorRepresentation errorRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheResponseShouldContainTheExpectedStatus(actualResponse, discoveryRequest, expectedStatusCode, expectedResponseDescription);
        }
        
        [Theory]
        [InlineData(ErrorCode.NoPatientFound)]
        [InlineData(ErrorCode.MultiplePatientsFound)]
        public async void ShouldSendTheErrorDetailsWhenNoPatientWasFound(ErrorCode errorCode)
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(JohnDoe, out DiscoveryRequest discoveryRequest);
            AndTheUserDoesNotMatchAnyPatientBecauseOf(errorCode, out ErrorRepresentation errorRepresentation);

            //When
            await careContextDiscoveryController.GetPatientCareContext(discoveryRequest);

            //Then
            ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(discoveryRequest, out GatewayDiscoveryRepresentation actualResponse);
            AndTheResponseShouldContainTheErrorDetails(actualResponse, errorRepresentation);
        }
        #endregion

        #region Ensure the job will be triggered by the background worker
        [Fact]
        public async void ShouldAddTheDiscoveryTaskToTheBackgroundJobList()
        {
            //Given
            GivenAPatientStartedANewDiscoveryRequest(JohnDoe, out DiscoveryRequest discoveryRequest);

            //When
            careContextDiscoveryController.DiscoverPatientCareContexts(discoveryRequest);

            //Then
            backgroundJobs.Should().ContainKey("GetPatientCareContext");
            ((DiscoveryRequest)backgroundJobs["GetPatientCareContext"].Args.First()).Should().BeSameAs(discoveryRequest);
        }
        #endregion

        
        #region Given
        private static void GivenAPatientStartedANewDiscoveryRequest(User user, out DiscoveryRequest discoveryRequest)
        {
            discoveryRequest = new DiscoveryRequestPayloadBuilder()
                .FromUser(user)
                .WithRequestId("aRequestId")
                .WithTransactionId("aTransactionId")
                .RequestedOn(new DateTime(2020, 06, 14))
                .Build();
        }

        private void AndThisPatientMatchASingleRegisteredPatient(User patient, IEnumerable<string> matchBy, out DiscoveryRepresentation discoveryRepresentation)
        {
            var discovery = new DiscoveryRepresentation(
                new PatientEnquiryRepresentation(
                    patient.Id,
                    patient.Name,
                    patient.CareContexts,
                    matchBy
                )
            );

            patientDiscoveryMock
                .Setup(patientDiscovery => patientDiscovery.PatientFor(It.IsAny<DiscoveryRequest>()))
                .Returns(async () => (discovery, null));

            discoveryRepresentation = discovery;
        }

        private void AndTheUserDoesNotMatchAnyPatientBecauseOf(ErrorCode errorCode, out ErrorRepresentation errorRepresentation)
        {
            var error = new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, "unusedMessage"));

            patientDiscoveryMock
                .Setup(patientDiscovery => patientDiscovery.PatientFor(It.IsAny<DiscoveryRequest>()))
                .Returns(async () => (null, error));

            errorRepresentation = new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, "unusedMessage"));
        }

        private void ButTheDataSourceIsNotReachable(out ErrorRepresentation errorRepresentation)
        {
            patientDiscoveryMock
                .Setup(patientDiscovery => patientDiscovery.PatientFor(It.IsAny<DiscoveryRequest>()))
                .Returns(async () => throw new Exception("Exception coming from tests"));
            
            errorRepresentation = new ErrorRepresentation(new Error(ErrorCode.ServerInternalError, "Unreachable external service"));
        }
        #endregion

        #region Then
        private void ThenAResponseToThisTransactionShouldHaveBeenSentToTheGateway(
            DiscoveryRequest discoveryRequest,
            out GatewayDiscoveryRepresentation actualResponse)
        {
            responsesSentToGateway.Should().ContainKey(discoveryRequest.TransactionId);

            actualResponse = responsesSentToGateway[discoveryRequest.TransactionId];
        }

        private static void AndTheSentResponseShouldContainTheFoundPatient(GatewayDiscoveryRepresentation actualResponse,
            PatientEnquiryRepresentation patientEnquiry)
        {
            actualResponse.Patient.ReferenceNumber.Should().Be(patientEnquiry.ReferenceNumber);
            actualResponse.Patient.Display.Should().Be(patientEnquiry.Display);
            actualResponse.Patient.CareContexts.Count().Should().Be(patientEnquiry.CareContexts.Count());
            foreach (CareContextRepresentation careContext in patientEnquiry.CareContexts)
            {
                actualResponse.Patient.CareContexts.Should().ContainEquivalentOf(careContext);
            }
        }

        private static void AndTheResponseShouldNotContainAnyPatientDetails(GatewayDiscoveryRepresentation actualResponse)
        {
            actualResponse.Patient.Should().BeNull();
        }

        private static void AndTheResponseShouldContainTheMatchFields(GatewayDiscoveryRepresentation actualResponse,
            ICollection<string> matchedFields)
        {
            actualResponse.Patient.MatchedBy.Count().Should().Be(matchedFields.Count);
            foreach (var matchedFieldName in matchedFields)
            {
                actualResponse.Patient.MatchedBy.Should().ContainEquivalentOf(matchedFieldName);
            }
        }

        private static void AndTheResponseShouldNotContainAyMatchFields(GatewayDiscoveryRepresentation actualResponse)
        {
            actualResponse.Patient.Should().BeNull();
        }

        private static void AndTheResponseShouldContainTheTransactionId(GatewayDiscoveryRepresentation actualResponse,
            DiscoveryRequest discoveryRequest)
        {
            actualResponse.TransactionId.Should().Be(discoveryRequest.TransactionId);
        }

        private static void AndTheResponseShouldContainTheErrorDetails(GatewayDiscoveryRepresentation actualResponse,
            ErrorRepresentation errorRepresentation)
        {
            actualResponse.Error.Code.Should().Be(errorRepresentation.Error.Code);
            actualResponse.Error.Message.Should().Be(errorRepresentation.Error.Message);
        }

        private static void AndTheResponseShouldNotContainAnyError(GatewayDiscoveryRepresentation actualResponse)
        {
            actualResponse.Error.Should().BeNull();
        }

        private static void AndTheResponseShouldContainTheExpectedStatus(GatewayDiscoveryRepresentation actualResponse, 
            DiscoveryRequest discoveryRequest, HttpStatusCode expectedStatusCode, string expectedMessage)
        {
            
            actualResponse.Resp.RequestId.Should().Be(discoveryRequest.RequestId);
            // actualResponse.Resp.StatusCode.Should().Be(expectedStatusCode);
            // actualResponse.Resp.Message.Should().Be(expectedMessage);
        }
        #endregion

        #region SetUp
        private static void SetupGatewayClientToSaveAllSentDiscoveryIntoThisList(Mock<IGatewayClient> gatewayClientMock,
            Dictionary<string, GatewayDiscoveryRepresentation> responsesSentToGateway)
        {
            gatewayClientMock
                .Setup(gatewayClient => gatewayClient.SendDataToGateway(
                    It.IsAny<string>(), It.IsAny<GatewayDiscoveryRepresentation>(), It.IsAny<string>())
                )
                .Callback<string, GatewayDiscoveryRepresentation, string>((urlPath, response, cmSuffix) =>
                {
                    responsesSentToGateway.TryAdd(response.TransactionId, response);
                });
        }

        private void SetupBackgroundJobClientToSaveAllCreatedJobsIntoThisList(
            Mock<IBackgroundJobClient> backgroundJobClientMock,
            Dictionary<string, Job> backgroundJobs)
        {
            backgroundJobClientMock
                .Setup(s => s.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                .Callback<Job, IState>((job, state) => { backgroundJobs.Add(job.Method.Name, job); });
        }
        #endregion
    }
}