namespace In.ProjectEKA.HipService.Link.Patient
{
    public class Session
    {
        public string SessionId { get; }
        public Communication Communication { get; }
        
        public Session(string sessionId, Communication communication)
        {
            SessionId = sessionId;
            Communication = communication;
        }
    }
}