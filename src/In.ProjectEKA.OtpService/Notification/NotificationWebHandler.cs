namespace In.ProjectEKA.OtpService.Notification
{
    using System.Collections.Specialized;
    using System.Net;
    using System.Web;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json.Linq;
    using Logger;

    public class NotificationWebHandler : INotificationWebHandler
    {
        private readonly IConfiguration configuration;

        public NotificationWebHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public NotificationResponse Send(string phoneNumber, string message)
        {
            var notification = HttpUtility.UrlEncode(message);
            using var wb = new WebClient();
            var response = wb.UploadValues("https://api.textlocal.in/send/",
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
            return (string)json["status"] == "success" ? new NotificationResponse(ResponseType.Success,"Notification sent") 
                : new NotificationResponse(ResponseType.InternalServerError, (string)json["errors"][0]["message"]);
        }
    }
}