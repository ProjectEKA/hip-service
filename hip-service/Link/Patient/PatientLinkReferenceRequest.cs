using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace hip_service.Link.Patient.Dto
{
    public class PatientLinkReferenceRequest
    {
        public string TransactionId { get; }
        
        public LinkReference Patient { get; }

        public PatientLinkReferenceRequest(string transactionId, LinkReference patient)
        {
            TransactionId = transactionId;
            Patient = patient;
        }
    }
}