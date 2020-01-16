namespace hip_service.Link.Patient
{
    public class Communication
    {
        public CommunicationMode Mode { get; }
        public string Value { get; }

        public Communication(CommunicationMode mode, string value)
        {
            Mode = mode;
            Value = value;
        }
    }
}