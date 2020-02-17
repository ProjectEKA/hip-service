namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("health-information/request")]
    public class DataFlowController : ControllerBase
    {
        private readonly IDataFlow dataFlow;

        public DataFlowController(IDataFlow dataFlow)
        {
            this.dataFlow = dataFlow;
        }

        [HttpPost]
        public async Task<ActionResult> HealthInformationRequestFor([FromBody] HealthInformationRequest healthInformationRequest)
        {
            var (healthInformationResponse, error) = await dataFlow
                .HealthInformationRequestFor(healthInformationRequest);
            return error != null ? ReturnServerResponse(error) : Ok(healthInformationResponse);
        }
        
        private ActionResult ReturnServerResponse(ErrorRepresentation errorResponse)
        {
            return errorResponse.Error.Code switch
            {
                ErrorCode.ServerInternalError => StatusCode(StatusCodes.Status500InternalServerError, errorResponse),
                ErrorCode.ContextArtefactIdNotFound => StatusCode(StatusCodes.Status404NotFound, errorResponse),
                _ => NotFound(errorResponse)
            };
        }
    }
}