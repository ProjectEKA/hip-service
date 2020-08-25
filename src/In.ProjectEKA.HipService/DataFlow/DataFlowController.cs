    namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using HipService.Gateway.Model;
    using Model;
    using Gateway;
    using Logger;
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
            [FromHeader(Name = "X-GatewayID")]
            string consentManagerId)
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
        private readonly IDataFlow dataFlow;
        private readonly IBackgroundJobClient backgroundJob;
        private readonly GatewayClient gatewayClient;

        public PatientDataFlowController(IDataFlow dataFlow, IBackgroundJobClient backgroundJob, GatewayClient gatewayClient)
        {
            this.dataFlow = dataFlow;
            this.backgroundJob = backgroundJob;
            this.gatewayClient = gatewayClient;
        }

        [HttpPost(PATH_HEALTH_INFORMATION_HIP_REQUEST)]
        public AcceptedResult HealthInformationRequestFor(PatientHealthInformationRequest healthInformationRequest,
                                                          [FromHeader(Name = "X-GatewayID")] string gatewayId)
        {
            backgroundJob.Enqueue(() => HealthInformationOf(healthInformationRequest, gatewayId));
            return Accepted();
        }

        [NonAction]
        public async Task HealthInformationOf(PatientHealthInformationRequest healthInformationRequest, string gatewayId)
        {
            try
            {
                var hiRequest = healthInformationRequest.HiRequest;
                var request = new HealthInformationRequest(healthInformationRequest.TransactionId,
                    hiRequest.Consent,
                    hiRequest.DateRange,
                    hiRequest.DataPushUrl,
                    hiRequest.KeyMaterial);
                var (response, error) = await dataFlow.HealthInformationRequestFor(request, gatewayId);
                var patientId = await dataFlow.GetPatientId(hiRequest.Consent.Id);
                var cmSuffix = patientId.Split("@")[1];
                var sessionStatus = DataFlowRequestStatus.ACKNOWLEDGED;
                if (error != null)
                {
                    sessionStatus = DataFlowRequestStatus.ERRORED;
                }

                var gatewayResponse = new GatewayDataFlowRequestResponse(
                    Guid.NewGuid(),
                    DateTime.Now.ToUniversalTime(),
                    new DataFlowRequestResponse(healthInformationRequest.TransactionId, sessionStatus.ToString()),
                    error?.Error,
                    new Resp(healthInformationRequest.RequestId));
                await gatewayClient.SendDataToGateway(PATH_HEALTH_INFORMATION_ON_REQUEST, gatewayResponse , cmSuffix);
            }  
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }
    }
}
