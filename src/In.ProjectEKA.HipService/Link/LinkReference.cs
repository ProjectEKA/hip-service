namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;

    [Obsolete]
    public class LinkReference
    {
        public string Id { get; }
        
        public string ReferenceNumber { get; }
        
        public IEnumerable<CareContextEnquiry> CareContexts { get; }

        public LinkReference(string id, string referenceNumber, IEnumerable<CareContextEnquiry> careContexts)
        {
            Id = id;
            ReferenceNumber = referenceNumber;
            CareContexts = careContexts;
        }
    }
}