namespace In.ProjectEKA.HipService.Link
{
    public class Session
    { 
        public string SessionId { get; }
        
        public Communication Communication { get; }
        public OtpGenerationDetail GenerationDetail { get; }
        
        public Session(string sessionId, Communication communication, OtpGenerationDetail generationDetail)
        {
            SessionId = sessionId;
            Communication = communication;
            GenerationDetail = generationDetail;
        }
    }
}