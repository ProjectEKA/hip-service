namespace In.ProjectEKA.HipService.DataFlow
{
    public class Consent
    {
        public string Id { get; }
        // ReSharper disable once InconsistentNaming
        private string DigitalSignature;

        public Consent(string id, string digitalSignature)
        {
            Id = id;
            DigitalSignature = digitalSignature;
        }
    }
}