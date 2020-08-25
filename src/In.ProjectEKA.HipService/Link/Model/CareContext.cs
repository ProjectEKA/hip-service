namespace In.ProjectEKA.HipService.Link.Model
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class CareContext
    {
        public CareContext()
        {
        }

        public CareContext(string careContextNumber)
        {
            CareContextNumber = careContextNumber;
        }

        public string CareContextNumber { get; set; }

        [ForeignKey("LinkReferenceNumber")]
        public string LinkReferenceNumber { get; set; }

        public LinkEnquires LinkEnquires { get; set; }
    }
}