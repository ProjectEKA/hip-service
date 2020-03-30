namespace In.ProjectEKA.HipService.Link
{
    public class OtpMessage
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string ResponseType { get; }

        public string Message { get; }

        public OtpMessage(string responseType, string message)
        {
            ResponseType = responseType;
            Message = message;
        }
    }
}