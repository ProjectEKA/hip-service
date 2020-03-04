namespace In.ProjectEKA.OtpService.Notification
{
    public class Communication
    {
        public CommunicationType CommunicationType { get; }
        public string Value { get; }

        public Communication(CommunicationType communicationType, string value)
        {
            CommunicationType = communicationType;
            Value = value;
        }
    }
}