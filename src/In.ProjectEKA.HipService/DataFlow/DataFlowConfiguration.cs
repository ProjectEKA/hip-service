namespace In.ProjectEKA.HipService.DataFlow
{
    public class DataFlowConfiguration
    {
        public int DataSizeLimitInMbs { get; set; }
        public int DataLinkTTLInMinutes { get; set; }

        public DataFlowConfiguration()
        {
        }
    }
}