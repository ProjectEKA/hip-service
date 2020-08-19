namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;

    [Obsolete]
    public class LinkReference
    {
        public LinkReference(string consentManagerUserId, string referenceNumber,
            IEnumerable<CareContextEnquiry> careContexts)
        {
            ConsentManagerUserId = consentManagerUserId;
            ReferenceNumber = referenceNumber;
            CareContexts = careContexts;
        }

        public string ConsentManagerUserId { get; }

        public string ReferenceNumber { get; }

        public IEnumerable<CareContextEnquiry> CareContexts { get; }
    }
}