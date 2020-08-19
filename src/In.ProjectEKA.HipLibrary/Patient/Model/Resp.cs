namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class Resp
    {
        public Resp(string requestId)
        {
            RequestId = requestId;
        }

        public string RequestId { get; }
    }
}