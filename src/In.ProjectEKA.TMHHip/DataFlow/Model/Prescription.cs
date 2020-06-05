using System;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class Prescription
    {
        public string PrescriptionId { get; set; }
        public string CaseNumber { get; set; }
        public DateTime Date { get; set; }
        public string ItemCode { get; set; }
        public string Medicine { get; set; }
        public int RequiredQuantity { get; set; }
        public int GivenQuantity { get; set; }
        public string Dosage { get; set; }
    }
}