namespace In.ProjectEKA.HipService.Link.Model
{
    using System.ComponentModel.DataAnnotations;

    public class InitiatedLinkRequest
    {
        [Key] public string RequestId { get; set; }

        public string TransactionId { get; set; }

        public string LinkReferenceNumber { get; set; }
        
        public bool Status { get; set; }
        
        public string DateTimeStamp { get; set; }

        public InitiatedLinkRequest(string requestId,
                                    string transactionId,
                                    string linkReferenceNumber,
                                    bool status,
                                    string dateTimeStamp)
        {
            RequestId = requestId;
            TransactionId = transactionId;
            LinkReferenceNumber = linkReferenceNumber;
            Status = status;
            DateTimeStamp = dateTimeStamp;
        }
    }
}