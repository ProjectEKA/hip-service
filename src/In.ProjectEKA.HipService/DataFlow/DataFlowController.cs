namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using HipLibrary.Patient.Model.Response;
    using Microsoft.AspNetCore.Http;
    
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
        
        private ActionResult ReturnServerResponse(ErrorResponse errorResponse)
        {
            return errorResponse.Error.Code switch
            {
                ErrorCode.ServerInternalError => StatusCode(StatusCodes.Status500InternalServerError, errorResponse),
                _ => NotFound(errorResponse)
            };
        }
    }
}