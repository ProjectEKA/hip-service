namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class LinkReferenceMeta
    {
        public string CommunicationMedium { get; set; }

        public string CommunicationHint { get; set; }

        public string CommunicationExpiry { get; }

        public LinkReferenceMeta(string communicationMedium, string communicationHint, string communicationExpiry)
        {
            CommunicationMedium = communicationMedium;
            CommunicationHint = communicationHint;
            CommunicationExpiry = communicationExpiry;
        }
    }
}