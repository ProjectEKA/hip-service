namespace In.ProjectEKA.HipService.DataFlow
{
    public class Consent
    {
        public string Id { get; }
        public string DigitalSignature { get; }

        public Consent(string id, string digitalSignature)
        {
            Id = id;
            DigitalSignature = digitalSignature;
        }
    }
}