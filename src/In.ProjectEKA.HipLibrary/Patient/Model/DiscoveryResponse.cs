using System.Net;

namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class DiscoveryResponse: Resp
    {
        public HttpStatusCode StatusCode { get; }
        public string Message { get; }

        public DiscoveryResponse(string requestId, HttpStatusCode statusCode, string message) : base(requestId)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}