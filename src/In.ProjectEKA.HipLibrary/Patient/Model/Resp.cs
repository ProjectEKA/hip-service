namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class Resp
    {
        public string RequestId { get; }

        public Resp(string requestId)
        {
            RequestId = requestId;
        }
    }
}