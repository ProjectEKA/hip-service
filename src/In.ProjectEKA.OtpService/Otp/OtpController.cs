namespace In.ProjectEKA.OtpService.Otp
{
    using System.Threading.Tasks;
    using Common;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("otp")]
    [ApiController]
    public class OtpController : Controller
    {
        private readonly OtpSenderFactory otpSenderFactory;
        private readonly OtpVerifier otpVerifier;

        public OtpController(OtpSenderFactory otpSenderFactory, OtpVerifier otpVerifier)
        {
            this.otpSenderFactory = otpSenderFactory;
            this.otpVerifier = otpVerifier;
        }

        [HttpPost]
        public async Task<ActionResult> GenerateOtp([FromBody] OtpGenerationRequest request)
        {
            var otpService = otpSenderFactory.ServiceFor(request?.Communication?.Value);
            var generateOtp = await otpService.GenerateOtp(request);
            return ResultFrom(generateOtp);
        }

        [HttpPost("{sessionId}/verify")]
        public async Task<ActionResult> VerifyOtp([FromRoute] string sessionId,
            [FromBody] OtpVerificationRequest request)
        {
            var verifyOtp = await otpVerifier.VerifyFor(sessionId, request.Value);
            return ResultFrom(verifyOtp);
        }

        private ActionResult ResultFrom(Response otpResponse)
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