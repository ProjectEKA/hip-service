namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model;
    using Matcher;
    using static RankBuilder;
    using static MetaBuilder;

    public class ExactNameRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string name)
        {
            return (ExactNameMatcher.IsMatch(patient.Name, name)) switch
            {
                true => new PatientWithRank<Patient>(patient, StrongMatchRank, FullMatchMeta(Match.Name)),
                _ => new PatientWithRank<Patient>(patient, EmptyRank, EmptyMeta),
            };
        }
    }
}