namespace In.ProjectEKA.OtpService.Otp
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    
    [Route("otp")]
    [ApiController]
    public class OtpController: Controller
    {
        private readonly IOtpService otpService;

        public OtpController(IOtpService otpService)
        {
            this.otpService = otpService;
        }

        [HttpPost]
        public async Task<ActionResult> GenerateOtp([FromBody] OtpGenerationRequest request)
        {
            var generateOtp = await otpService.GenerateOtp(request);
            return ReturnServerResponse(generateOtp);
        }

        [HttpPost("{sessionId}/verify")]
        public async Task<ActionResult> VerifyOtp([FromRoute] string sessionId, [FromBody] OtpVerificationRequest request)
        {
            var verifyOtp = await otpService.CheckOtpValue(sessionId, request.Value);
            return ReturnServerResponse(verifyOtp);
        }

        private ActionResult ReturnServerResponse(OtpResponse otpResponse)
        {
            return otpResponse.ResponseType switch
            {
                ResponseType.Success => (ActionResult) Ok(otpResponse),
                ResponseType.OtpValid => Ok(otpResponse),
                ResponseType.OtpInvalid => BadRequest(otpResponse),
                ResponseType.OtpExpired => Unauthorized(otpResponse),
                ResponseType.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, otpResponse),
                _ => NotFound(otpResponse)
            };
        }
    }
}