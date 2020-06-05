using System.Collections;
using System.Collections.Generic;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class PatientData
    {
        public List<ClinicalNote> ClinicalNotes { get; set; }
        public List<Prescription> Prescriptions { get; set; }
    }
}