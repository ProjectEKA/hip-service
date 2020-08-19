namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class Error
    {
        public Error(ErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }

        public ErrorCode Code { get; }

        public string Message { get; }
    }
}