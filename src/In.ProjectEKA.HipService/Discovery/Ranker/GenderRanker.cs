namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model;
    using static RankBuilder;
    using static MetaBuilder;

    public class GenderRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string gender)
        {
            return patient.Gender?.ToString() == gender
                ? new PatientWithRank<Patient>(patient, WeakMatchRank, FullMatchMeta(Match.Gender))
                : new PatientWithRank<Patient>(patient, EmptyRank, EmptyMeta);
        }
    }
}