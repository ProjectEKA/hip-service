namespace In.ProjectEKA.HipService.Link
{
    public class Session
    {
        public Session(string sessionId, Communication communication, OtpGenerationDetail generationDetail)
        {
            SessionId = sessionId;
            Communication = communication;
            GenerationDetail = generationDetail;
        }

        public string SessionId { get; }

        public Communication Communication { get; }
        public OtpGenerationDetail GenerationDetail { get; }
    }
}