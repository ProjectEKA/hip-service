namespace In.ProjectEKA.HipService.Link.Patient.Model
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class LinkedCareContext
    {
        public LinkedCareContext()
        {
        }

        public LinkedCareContext(string careContextNumber)
        {
            CareContextNumber = careContextNumber;
        }

        public string CareContextNumber { get; set; }

        [ForeignKey("LinkReferenceNumber")] public string LinkReferenceNumber { get; set; }

        public LinkRequest LinkRequest { get; set; }
    }
}