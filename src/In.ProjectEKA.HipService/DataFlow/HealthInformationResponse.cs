namespace In.ProjectEKA.HipService.DataFlow
{
    public class HealthInformationResponse
    {
        public string Content { get; }

        public HealthInformationResponse(string content)
        {
            Content = content;
        }
    }
}