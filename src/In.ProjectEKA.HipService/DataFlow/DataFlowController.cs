namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Gateway;
    using Gateway.Model;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Model;
    using static Common.Constants;

    [ApiController]
    public class DataFlowController : ControllerBase
    {
        private readonly IDataFlow dataFlow;

        public DataFlowController(IDataFlow dataFlow)
        {
            this.dataFlow = dataFlow;
        }

        [Obsolete]
        [Authorize]
        [HttpPost]
        [Route("health-information/request")]
        public async Task<ActionResult> HealthInformationRequestFor(
            [FromBody] HealthInformationRequest healthInformationRequest,
            [FromHeader(Name = "X-GatewayID")] string consentManagerId)
        {
            var (healthInformationResponse, error) = await dataFlow
                .HealthInformationRequestFor(healthInformationRequest, consentManagerId);
            return error != null ? ServerResponseFor(error) : Ok(healthInformationResponse);
        }

        [HttpGet]
        [Route("health-information/{informationId}")]
        public async Task<ActionResult> HealthInformation([FromRoute] string informationId, [FromQuery] string token)
        {
            var (healthInformation, error) = await dataFlow.HealthInformationFor(informationId, token);
            return error != null ? ServerResponseFor(error) : Ok(healthInformation);
        }

        private ActionResult ServerResponseFor(ErrorRepresentation errorResponse)
        {
            return errorResponse.Error.Code switch
            {
                ErrorCode.ServerInternalError => StatusCode(StatusCodes.Status500InternalServerError, errorResponse),
                ErrorCode.ContextArtefactIdNotFound => StatusCode(StatusCodes.Status404NotFound, errorResponse),
                ErrorCode.InvalidToken => StatusCode(StatusCodes.Status403Forbidden, errorResponse),
                ErrorCode.HealthInformationNotFound => StatusCode(StatusCodes.Status404NotFound, errorResponse),
                ErrorCode.LinkExpired => StatusCode(StatusCodes.Status403Forbidden, errorResponse),
                ErrorCode.ExpiredKeyPair => StatusCode(StatusCodes.Status400BadRequest, errorResponse),
                _ => Problem(errorResponse.Error.Message)
                };
        }
    }

    [ApiController]
    public class PatientDataFlowController : ControllerBase
    {
        private readonly IBackgroundJobClient backgroundJob;
        private readonly IDataFlow dataFlow;
        private readonly GatewayClient gatewayClient;
        private readonly GatewayConfiguration gatewayConfiguration;
        private readonly ILogger<PatientDataFlowController> logger;

        public PatientDataFlowController(IDataFlow dataFlow,
            IBackgroundJobClient backgroundJob,
            GatewayClient gatewayClient,
            GatewayConfiguration gatewayConfiguration,
            ILogger<PatientDataFlowController> logger)
        {
            this.dataFlow = dataFlow;
            this.backgroundJob = backgroundJob;
            this.gatewayClient = gatewayClient;
            this.logger = logger;
            this.gatewayConfiguration = gatewayConfiguration;
        }

        [HttpPost(PATH_HEALTH_INFORMATION_HIP_REQUEST)]
        public AcceptedResult HealthInformationRequestFor(PatientHealthInformationRequest healthInformationRequest,
            [FromHeader(Name = CORRELATION_ID)] string correlationId, 
            [FromHeader(Name = "X-GatewayID")] string gatewayId)
        {
            logger.Log(LogLevel.Information, LogEvents.DataFlow, "Data request received");
            backgroundJob.Enqueue(() => HealthInformationOf(healthInformationRequest, gatewayId, correlationId));
            return Accepted();
        }

        [NonAction]
        public async Task HealthInformationOf(PatientHealthInformationRequest healthInformationRequest,
            string gatewayId, string correlationId)
        {
            try
            {
                var hiRequest = healthInformationRequest.HiRequest;
                var request = new HealthInformationRequest(healthInformationRequest.TransactionId,
                    hiRequest.Consent,
                    hiRequest.DateRange,
                    hiRequest.DataPushUrl,
                    hiRequest.KeyMaterial);
                var (_, error) = await dataFlow.HealthInformationRequestFor(request, gatewayId);
                GatewayDataFlowRequestResponse gatewayResponse;
                if (error != null)
                {
                    gatewayResponse = new GatewayDataFlowRequestResponse(
                        Guid.NewGuid(),
                        DateTime.Now.ToUniversalTime(),
                        new DataFlowRequestResponse(healthInformationRequest.TransactionId,
                            DataFlowRequestStatus.ERRORED.ToString()),
                        error.Error,
                        new Resp(healthInformationRequest.RequestId));
                    logger.Log(LogLevel.Error,
                        LogEvents.DataFlow,
                        "Response for data request {@GatewayResponse}",
                        gatewayResponse);
                }
                else
                {
                    gatewayResponse = new GatewayDataFlowRequestResponse(
                        Guid.NewGuid(),
                        DateTime.Now.ToUniversalTime(),
                        new DataFlowRequestResponse(healthInformationRequest.TransactionId,
                            DataFlowRequestStatus.ACKNOWLEDGED.ToString()),
                        null,
                        new Resp(healthInformationRequest.RequestId));
                    logger.Log(LogLevel.Information,
                        LogEvents.DataFlow,
                        "Response for data request {@GatewayResponse}",
                        gatewayResponse);
                }
                await gatewayClient.SendDataToGateway(PATH_HEALTH_INFORMATION_ON_REQUEST,
                    gatewayResponse,
                    gatewayConfiguration.CmSuffix, correlationId);
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error,
                    LogEvents.DataFlow,
                    exception,
                    "Error happened when responding gateway");
            }
        }
    }
}