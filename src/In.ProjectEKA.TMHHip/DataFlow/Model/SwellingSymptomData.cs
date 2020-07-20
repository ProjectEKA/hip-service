using System;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class SwellingSymptomData
    {
        public string CaseNumber { get; set; }
        public Decimal HstExmNo { get; set; }
        public Decimal SymptNo { get; set; }
        public DateTime RecordedDate { get; set; }
        public string SwellingSize { get; set; }
        public string SwellingLtrl { get; set; }
        public string SwellingSite { get; set; }
    }
}