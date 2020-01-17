namespace In.ProjectEKA.HipService.Link.Patient
{
    public class Session
    {
        public Session(string sessionId, Communication communication)
        {
            SessionId = sessionId;
            Communication = communication;
        }

        public string SessionId { get; }
        private Communication Communication { get; }
    }
}