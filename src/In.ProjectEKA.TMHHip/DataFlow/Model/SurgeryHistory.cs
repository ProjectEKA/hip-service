using System;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class SurgeryHistory
    {
        public string CaseNumber { get; set; }
        public Decimal HstExmNo { get; set; }
        public DateTime SurgeryWhen { get; set; }
        public string SurgeryDtls { get; set; }
        public string HospitalDtls { get; set; }
        public string SurgeryRemarks { get; set; }
    }
}