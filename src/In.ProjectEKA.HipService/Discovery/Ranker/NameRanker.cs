namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model;
    using Matcher;
    using static RankBuilder;
    using static MetaBuilder;

    public class NameRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string name)
        {
            var diff = FuzzyNameMatcher.LevenshteinDistance(patient.Name, name);
            if (diff == 0)
                return new PatientWithRank<Patient>(patient, StrongMatchRank, FullMatchMeta(Match.Name));

            return diff <= 2
                ? new PatientWithRank<Patient>(patient, new Rank(8), FullMatchMeta(Match.Name))
                : new PatientWithRank<Patient>(patient, EmptyRank, EmptyMeta);
        }
    }
}