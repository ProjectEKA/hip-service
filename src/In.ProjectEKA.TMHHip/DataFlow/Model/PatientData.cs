using System.Collections.Generic;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class PatientData
    {
        public List<ClinicalNote> ClinicalNotes { get; set; }
        public List<Prescription> Prescriptions { get; set; }
        public List<AbdomenExaminationData> AbdomenExaminationsData { get; set; }
        public List<OralCavityExaminationData> OralCavityExaminationsData { get; set; }
        public List<SurgeryHistory> SurgeryHistories { get; set; }
        public List<AllergyData> AllergiesData { get; set; }
        public List<SwellingSymptomData> SwellingSymptomsData { get; set; }
    }
}