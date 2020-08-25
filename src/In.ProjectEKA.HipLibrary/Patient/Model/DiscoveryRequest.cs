using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System;

    public class DiscoveryRequest
    {
        public PatientEnquiry Patient { get; }

        public string RequestId { get; }

        [Required, MaxLength(50)]
        public string TransactionId { get; }

        public DateTime Timestamp { get; }

        public DiscoveryRequest(PatientEnquiry patient, string requestId, string transactionId, DateTime timestamp)
        {
            Patient = patient;
            RequestId = requestId;
            TransactionId = transactionId;
            Timestamp = timestamp;
        }
    }
}