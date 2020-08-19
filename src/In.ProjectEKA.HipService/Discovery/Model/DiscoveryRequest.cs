// ReSharper disable All

namespace In.ProjectEKA.HipService.Discovery.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics.CodeAnalysis;

    public class DiscoveryRequest
    {
        public DiscoveryRequest(string transactionId, string consentManagerUserId, string patientReferenceNumber)
        {
            TransactionId = transactionId;
            ConsentManagerUserId = consentManagerUserId;
            PatientReferenceNumber = patientReferenceNumber;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string TransactionId { get; set; }

        [Required, MaxLength(50)]
        public string ConsentManagerUserId { get; set; }

        public DateTime Timestamp { get; set; }

        [Required, MaxLength(50)]
        public string PatientReferenceNumber { get; set; }
    }
}