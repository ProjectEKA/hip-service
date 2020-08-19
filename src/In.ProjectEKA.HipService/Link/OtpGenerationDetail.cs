namespace In.ProjectEKA.HipService.Link
{
    public class OtpGenerationDetail
    {
        public OtpGenerationDetail(string systemName, string action)
        {
            SystemName = systemName;
            Action = action;
        }

        public string SystemName { get; set; }
        public string Action { get; set; }
    }
}