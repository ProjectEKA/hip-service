namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model;

    public class GenderRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string gender)
        {
            return patient.Gender == gender
                ? new PatientWithRank<Patient>(patient, RankBuilder.WeakMatchRank,
                    MetaBuilder.FullMatchMeta(Match.Gender))
                : new PatientWithRank<Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}