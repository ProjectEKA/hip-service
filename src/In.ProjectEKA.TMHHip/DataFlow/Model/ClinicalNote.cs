using System;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class ClinicalNote
    {
        public Decimal NoteNumber { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { set; get; }
        public string UserName { set; get; }
    }
}