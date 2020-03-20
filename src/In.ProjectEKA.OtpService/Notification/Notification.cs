namespace In.ProjectEKA.OtpService.Notification
{
    using Newtonsoft.Json.Linq;

    public class Notification
    {
        public string Id { get; }
        public Communication Communication { get; }
        public JObject Content { get; }
        public Action Action { get; }

        public Notification(string id, Communication communication, JObject content, Action action)
        {
            Id = id;
            Communication = communication;
            Content = content;
            Action = action;
        }
    }
}