namespace In.ProjectEKA.HipService.Link
{
    public class OtpMessage
    {
        public string ResponseType { get; }
        
        public string Message { get; }

        public OtpMessage(string responseType, string message)
        {
            ResponseType = responseType;
            Message = message;
        }
    }
}