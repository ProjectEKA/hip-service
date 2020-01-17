using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace In.ProjectEKA.HipService.Discovery.Patient.Model
{
    public class DiscoveryRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)] public string TransactionId { get; set; }
        [Required, MaxLength(50)] public string ConsentManagerUserId { get; set; }
        public DateTime Timestamp { get; set; }

        public DiscoveryRequest(string transactionId, string consentManagerUserId)
        {
            TransactionId = transactionId;
            ConsentManagerUserId = consentManagerUserId;
        }
    }
}