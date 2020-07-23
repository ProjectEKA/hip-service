namespace In.ProjectEKA.HipService.Link
{
    public class OtpGenerationDetail
    {
        public string SystemName { get; set; }
        public string Action { get; set; }

        public OtpGenerationDetail(string systemName, string action)
        {
            SystemName = systemName;
            Action = action;
        }
    }
}