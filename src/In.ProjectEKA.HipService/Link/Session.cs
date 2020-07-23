namespace In.ProjectEKA.HipService.Link
{
    public class Session
    { 
        public string SessionId { get; }
        
        public Communication Communication { get; }
        public OtpCreationDetail CreationDetail { get; }
        
        public Session(string sessionId, Communication communication, OtpCreationDetail creationDetail)
        {
            SessionId = sessionId;
            Communication = communication;
            CreationDetail = creationDetail;
        }
    }
}