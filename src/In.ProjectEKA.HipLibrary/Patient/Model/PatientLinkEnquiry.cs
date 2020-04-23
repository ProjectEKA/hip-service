namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class PatientLinkEnquiry
    {
        public string TransactionId { get; }

        public string RequestId { get; }

        public LinkEnquiry Patient { get; }

        public PatientLinkEnquiry(string transactionId, string requestId, LinkEnquiry patient)
        {
            TransactionId = transactionId;
            Patient = patient;
            RequestId = requestId;
        }
    }
}