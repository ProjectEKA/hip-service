using System.ComponentModel.DataAnnotations.Schema;

namespace In.ProjectEKA.DefaultHip.Link.Model
{
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