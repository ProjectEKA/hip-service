namespace In.ProjectEKA.HipService.Link.Patient
{
    public class Communication
    {
        private readonly CommunicationMode Mode;
        private readonly string Value;

        public Communication(CommunicationMode mode, string value)
        {
            Mode = mode;
            Value = value;
        }
    }
}