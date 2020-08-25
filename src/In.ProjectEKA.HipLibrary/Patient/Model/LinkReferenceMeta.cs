namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class LinkReferenceMeta
    {
        public LinkReferenceMeta(string communicationMedium, string communicationHint, string communicationExpiry)
        {
            CommunicationMedium = communicationMedium;
            CommunicationHint = communicationHint;
            CommunicationExpiry = communicationExpiry;
        }

        public string CommunicationMedium { get; }

        public string CommunicationHint { get; }

        public string CommunicationExpiry { get; }
    }
}