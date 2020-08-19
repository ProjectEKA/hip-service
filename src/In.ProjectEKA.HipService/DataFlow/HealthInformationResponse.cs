namespace In.ProjectEKA.HipService.DataFlow
{
    public class HealthInformationResponse
    {
        public HealthInformationResponse(string content)
        {
            Content = content;
        }

        public string Content { get; }
    }
}