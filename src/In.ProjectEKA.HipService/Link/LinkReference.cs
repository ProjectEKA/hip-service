namespace In.ProjectEKA.HipService.Link
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;

    public class LinkReference
    {
        public string ConsentManagerUserId { get; }
        
        public string ReferenceNumber { get; }
        
        public IEnumerable<CareContextEnquiry> CareContexts { get; }

        public LinkReference(string consentManagerUserId, string referenceNumber, IEnumerable<CareContextEnquiry> careContexts)
        {
            ConsentManagerUserId = consentManagerUserId;
            ReferenceNumber = referenceNumber;
            CareContexts = careContexts;
        }
    }
}