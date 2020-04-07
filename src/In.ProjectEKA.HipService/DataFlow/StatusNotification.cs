namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Collections.Generic;

    public class StatusNotification
    {
        public SessionStatus SessionStatus { get; }
        public string HipId { get; }
        public IEnumerable<StatusResponse> StatusResponses { get; }
        
        public StatusNotification(SessionStatus sessionStatus, string hipId, IEnumerable<StatusResponse> statusResponses)
        {
            SessionStatus = sessionStatus;
            HipId = hipId;
            StatusResponses = statusResponses;
        }
    }
}