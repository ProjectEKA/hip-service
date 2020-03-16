namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class PatientLinkEnquiry
    {
        public string TransactionId { get; }

        public LinkEnquiry Patient { get; }

        public PatientLinkEnquiry(string transactionId, LinkEnquiry patient)
        {
            TransactionId = transactionId;
            Patient = patient;
        }
    }
}