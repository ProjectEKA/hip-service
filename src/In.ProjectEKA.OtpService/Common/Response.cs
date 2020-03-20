namespace In.ProjectEKA.OtpService.Common
{
    public class Response
    {
        public ResponseType ResponseType { get; }
        public string Message { get; }
        
        public Response(ResponseType responseType, string message)
        {
            ResponseType = responseType;
            Message = message;
        }
    }
}