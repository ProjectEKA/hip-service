namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    public class DataFlowController : ControllerBase
    {
        private readonly IDataFlow dataFlow;

        public DataFlowController(IDataFlow dataFlow)
        {
            this.dataFlow = dataFlow;
        }

        [Authorize]
        [HttpPost]
        [Route("health-information/request")]
        public async Task<ActionResult> HealthInformationRequestFor(
            [FromBody] HealthInformationRequest healthInformationRequest,
            [FromHeader(Name = "X-GatewayID ")]
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
    [Route("/v1/health-information/hip")]
    public class PatientDataFlowController : ControllerBase
    {
        private readonly IDataFlow dataFlow;
        private readonly IBackgroundJobClient backgroundJob;

        public PatientDataFlowController(IDataFlow dataFlow, IBackgroundJobClient backgroundJob)
        {
            this.dataFlow = dataFlow;
            this.backgroundJob = backgroundJob;
        }

        [Route("request")]
        [HttpPost]
        public AcceptedResult HealthInformationRequestFor(PatientHealthInformationRequest healthInformationRequest,
                                                          [FromHeader(Name = "X-GatewayID")] string gatewayId)
        {
            backgroundJob.Enqueue(() => HealthInformationOf(healthInformationRequest, gatewayId));
            return Accepted();
        }

        public async Task HealthInformationOf(PatientHealthInformationRequest healthInformationRequest, string gatewayId)
        {
            var hiRequest = healthInformationRequest.HiRequest;
            var request = new HealthInformationRequest(healthInformationRequest.TransactionId,
                hiRequest.Consent,
                hiRequest.DateRange,
                hiRequest.DataPushUrl,
                hiRequest.KeyMaterial);

            await dataFlow.HealthInformationRequestFor(request, gatewayId);

            //TODO:: OnRequest implementation pending
        }
    }
}