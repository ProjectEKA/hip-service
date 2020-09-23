namespace In.ProjectEKA.FHIRHip.DataFlow.Model
{
    public class DataFlowConfiguration
    {
        public int DataSizeLimitInMbs { get; set; }
        public int DataLinkTtlInMinutes { get; set; }
        public string Url { get; set; }
        public bool IsAuthEnabled { get; set; }
        public string AuthUrl { get; set; }
    }
}