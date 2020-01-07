using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using HipLibrary.Patient.Models.Request;

namespace hip_service.Link.Patient.Dto
{
    public class LinkReference
    {
        public string ConsentManagerUserId { get; }
        
        public string ReferenceNumber { get; }
        
        public IEnumerable<CareContext> CareContexts { get; }

        public LinkReference(string consentManagerUserId, string referenceNumber, IEnumerable<CareContext> careContexts)
        {
            ConsentManagerUserId = consentManagerUserId;
            ReferenceNumber = referenceNumber;
            CareContexts = careContexts;
        }
    }
}