namespace In.ProjectEKA.OtpService.Clients
{
    using System.Collections.Specialized;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json.Linq;
    using Notification.Logger;

    public class SmsClient : ISmsClient
    {
        private readonly IConfiguration configuration;

        public SmsClient(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<Response> Send(string phoneNumber, string message)
        {
            var notification = HttpUtility.UrlEncode(message);
            using var webClient = new WebClient();
            var response = await webClient.UploadValuesTaskAsync("https://api.textlocal.in/send/",
                new NameValueCollection
                {
                    {"apikey" , configuration.GetConnectionString("TextLocaleApiKey")},
                    {"numbers" , phoneNumber},
                    {"message" , notification},
                    {"sender name" , "HCMNCG"},
                });
            var json = JObject.Parse(System.Text.Encoding.UTF8.GetString(response));
            Log.Information((string)json["status"] == "success" ? "Success in sending notification" :
                (string)json["errors"][0]["message"]);
            return (string)json["status"] == "success" ?  new Response(ResponseType.Success,"Notification sent") 
                    :new Response(ResponseType.Success, (string)json["errors"][0]["message"]);
        }
    }
}
