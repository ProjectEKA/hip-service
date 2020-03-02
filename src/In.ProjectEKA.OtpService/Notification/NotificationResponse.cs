namespace In.ProjectEKA.OtpService.Notification
{
    public class NotificationResponse
    {
        public ResponseType ResponseType { get; }
        public string Message { get; }

        public NotificationResponse(ResponseType responseType, string message)
        {
            ResponseType = responseType;
            Message = message;
        }
    }
}