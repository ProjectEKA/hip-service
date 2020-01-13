namespace otp_server.Otp
{
    public class OtpResponse
    {
        public ResponseType ResponseType { get; }
        public string Message { get; }

        public OtpResponse(ResponseType responseType, string message)
        {
            ResponseType = responseType;
            Message = message;
        }
    }
}