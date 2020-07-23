namespace In.ProjectEKA.HipService.Link
{
    public class OtpCreationDetail
    {
        public string SystemName { get; set; }
        public string Action { get; set; }

        public OtpCreationDetail(string systemName, string action)
        {
            SystemName = systemName;
            Action = action;
        }
    }
}