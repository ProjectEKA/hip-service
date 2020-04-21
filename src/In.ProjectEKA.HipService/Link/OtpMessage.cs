namespace In.ProjectEKA.HipService.Link
{
    using In.ProjectEKA.HipLibrary.Patient.Model;

    public class OtpMessage
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ResponseType ResponseType { get; }

        public string Message { get; }

        public OtpMessage(ResponseType responseType, string message)
        {
            ResponseType = responseType;
            Message = message;
        }

        public Error toError()
        {
            return ResponseType switch
            {
                ResponseType.OtpInvalid => new Error(ErrorCode.OtpInValid, Message),
                ResponseType.OtpExpired => new Error(ErrorCode.OtpExpired, Message),
                _ => new Error(ErrorCode.ServerInternalError, Message)
            };
        }
    }
}