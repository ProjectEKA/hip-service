using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace otp_server.Otp
{
    public class OtpService
    {
        private readonly IOtpRepository otpRepository;
        
        public OtpService(IOtpRepository otpRepository)
        {
            this.otpRepository = otpRepository;
        }

        public async Task<OtpResponse> GenerateOtp(OtpGenerationRequest otpGenerationRequest)
        {
            var otpValue = OtpGenerator();
            var sendOtp = SendOtp(otpGenerationRequest.Communication.Value, otpValue);
            if (sendOtp.ResponseType == ResponseType.Success)
            {
                return await otpRepository.Save(otpValue,otpGenerationRequest.SessionId);
            }
            return sendOtp;
        }
        
        public async Task<OtpResponse> CheckOtpValue(string sessionId, string value)
        {
            var otpRequest = await otpRepository.GetOtp(sessionId) ;
            return otpRequest.Map(o => o.OtpToken == value
                ? new OtpResponse(ResponseType.OtpValid,"Valid OTP")
                : new OtpResponse(ResponseType.OtpInvalid,"Invalid Otp"))
                .ValueOr(new OtpResponse(ResponseType.InternalServerError,"Session Id Not Found"));
        }
        
        private static OtpResponse SendOtp(string value, string otp)
        {
            var message = HttpUtility.UrlEncode("Your Otp is: " + otp);
            using var wb = new WebClient();
            var response =  wb.UploadValues("https://api.textlocal.in/send/",
                new NameValueCollection()
            {
                {"apikey" , "IwCFl4LRc+8-J2PvpYTfQ46hotNN0ztNhPMgOeCWYl"},
                {"numbers" , value},
                {"message" , message},
                {"sender name" , "HCMNCG"},
            });
            var json = JObject.Parse(System.Text.Encoding.UTF8.GetString(response));
            return (string)json["status"] == "success" ? new OtpResponse(ResponseType.Success,"OTP created") 
                : new OtpResponse(ResponseType.InternalServerError, (string)json["errors"][0]["message"]);
        }

        private static string OtpGenerator()
        {
            const string chars1 = "1234567890";
            var stringChars1 = new char[6];
            var random1 = new Random();
            for (var i = 0; i < stringChars1.Length; i++)
            {
                stringChars1[i] = chars1[random1.Next(chars1.Length)];
            }
            return new string(stringChars1);
        }
    }
}