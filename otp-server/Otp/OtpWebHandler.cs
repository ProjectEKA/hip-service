using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace otp_server.Otp
{
    public class OtpWebHandler: IOtpWebHandler
    {
        public OtpResponse SendOtp(string value, string otp)
        {
            var message = HttpUtility.UrlEncode("Your Otp is: " + otp);
            using var wb = new WebClient();
            var response = wb.UploadValues("https://api.textlocal.in/send/",
                new NameValueCollection
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
    }
}