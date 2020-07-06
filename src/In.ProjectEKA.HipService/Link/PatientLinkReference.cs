namespace In.ProjectEKA.HipService.Link
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;

    public class PatientLinkReference
    {
        public string Id { get; }
        
        public string ReferenceNumber { get; set; }

        public IEnumerable<CareContextEnquiry> CareContexts { get; set; }

        public PatientLinkReference(string id, string referenceNumber, IEnumerable<CareContextEnquiry> careContexts)
        {
            Id = id;
            ReferenceNumber = referenceNumber;
            CareContexts = careContexts;
        }
    }
}