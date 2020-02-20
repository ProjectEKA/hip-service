namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
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

        [HttpPost]
        [Route("health-information/request")]
        public async Task<ActionResult> HealthInformationRequestFor(
            [FromBody] HealthInformationRequest healthInformationRequest)
        {
            var (healthInformationResponse, error) = await dataFlow
                .HealthInformationRequestFor(healthInformationRequest);
            return error != null ? ReturnServerResponse(error) : Ok(healthInformationResponse);
        }

        [HttpGet]
        [Route("health-information/{linkId}")]
        public async Task<ActionResult> HealthInformation(
            [FromRoute] string linkId,
            [FromQuery] string token,
            [FromQuery] string transactionId)
        {
            var (healthInformationResponse, error) = await dataFlow.HealthInformationFor(linkId, token, transactionId);
            return error != null ? ReturnServerResponse(error) : Ok(healthInformationResponse);
        }

        private ActionResult ReturnServerResponse(ErrorRepresentation errorResponse)
        {
            return errorResponse.Error.Code switch
            {
                ErrorCode.ServerInternalError => StatusCode(StatusCodes.Status500InternalServerError, errorResponse),
                ErrorCode.ContextArtefactIdNotFound => StatusCode(StatusCodes.Status404NotFound, errorResponse),
                ErrorCode.InvalidToken => StatusCode(StatusCodes.Status403Forbidden, errorResponse),
                ErrorCode.LinkDataNotFound => StatusCode(StatusCodes.Status404NotFound, errorResponse),
                ErrorCode.LinkExpired => StatusCode(StatusCodes.Status403Forbidden, errorResponse),
                _ => Problem(errorResponse.Error.Message)
            };
        }
    }
}