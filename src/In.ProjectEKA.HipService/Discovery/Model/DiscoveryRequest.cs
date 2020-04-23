// ReSharper disable All

namespace In.ProjectEKA.HipService.Discovery.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics.CodeAnalysis;

    public class DiscoveryRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)] public string RequestId { get; set; }

        [Required, MaxLength(50)] public string ConsentManagerUserId { get; set; }

        public DateTime Timestamp { get; set; }

        [Required, MaxLength(50)] public string PatientReferenceNumber { get; set; }

        public DiscoveryRequest(string requestId, string consentManagerUserId, string patientReferenceNumber)
        {
            RequestId = requestId;
            ConsentManagerUserId = consentManagerUserId;
            PatientReferenceNumber = patientReferenceNumber;
        }
    }
}