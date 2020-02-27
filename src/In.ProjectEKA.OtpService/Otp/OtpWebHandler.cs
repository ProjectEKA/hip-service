namespace In.ProjectEKA.OtpService.Otp
{
    using System.Collections.Specialized;
    using System.Net;
    using System.Web;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json.Linq;
    
    public class OtpWebHandler: IOtpWebHandler
    {
        private readonly IConfiguration configuration;

        public OtpWebHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public OtpResponse SendOtp(string phoneNumber, string otp)
        {
            var message = HttpUtility.UrlEncode("Your Otp is: " + otp);
            using var wb = new WebClient();
            var response = wb.UploadValues("https://api.textlocal.in/send/",
                new NameValueCollection
                {
                    {"apikey" , configuration.GetConnectionString("TextLocaleApiKey")},
                    {"numbers" , phoneNumber},
                    {"message" , message},
                    {"sender name" , "HCMNCG"},
                });
            var json = JObject.Parse(System.Text.Encoding.UTF8.GetString(response));
            return (string)json["status"] == "success" ? new OtpResponse(ResponseType.Success,"OTP created") 
                : new OtpResponse(ResponseType.InternalServerError, (string)json["errors"][0]["message"]);
        }
    }
}