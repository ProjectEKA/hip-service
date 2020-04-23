namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
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
            [FromHeader(Name = "X-ConsentManagerId")] string consentManagerId)
        {
            var (healthInformationResponse, error) = await dataFlow
                .HealthInformationRequestFor(healthInformationRequest, consentManagerId);
            return error != null ? ServerResponseFor(error) : Ok(healthInformationResponse);
        }

        [HttpGet]
        [Route("health-information/{informationId}")]
        public async Task<ActionResult> HealthInformation(
            [FromRoute] string informationId,
            [FromQuery] string token)
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
}