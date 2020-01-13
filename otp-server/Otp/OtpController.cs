using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace otp_server.Otp
{
    [Route("otp")]
    [ApiController]
    public class OtpController: Controller
    {
        private readonly OtpService otpService;

        public OtpController(OtpService otpService)
        {
            this.otpService = otpService;
        }

        [HttpPost("link")]
        public async Task<ActionResult> GenerateOtp([FromBody] OtpGenerationRequest request)
        {
            var generateOtp = await otpService.GenerateOtp(request);
            return ReturnServerResponse(generateOtp);
        }

        [HttpPost("verify")]
        public async Task<ActionResult> VerifyOtp([FromBody] OtpVerificationRequest request)
        {
            var verifyOtp = await otpService.CheckOtpValue(request.SessionID, request.Value);
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
                ResponseType.OtpGenerationFailed => BadRequest(otpResponse),
                ResponseType.InternalServerError => BadRequest(otpResponse),
                _ => NotFound(otpResponse)
            };
        }
    }
}