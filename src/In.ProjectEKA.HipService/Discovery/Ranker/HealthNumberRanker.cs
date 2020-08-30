namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model;
    using static RankBuilder;
    using static MetaBuilder;
    
    public class HealthNumberRanker
    {
        public PatientWithRank<Patient> Rank(Patient patient, string healthNumber)
        {
            return patient.HealthNumber == healthNumber
                ? new PatientWithRank<Patient>(patient, StrongMatchRank, FullMatchMeta(Match.NdhmHealthNumber))
                : new PatientWithRank<Patient>(patient, EmptyRank, EmptyMeta);
        }
    }
}