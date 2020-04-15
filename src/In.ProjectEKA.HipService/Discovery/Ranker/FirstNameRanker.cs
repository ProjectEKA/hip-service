namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model;
    using Matcher;

    public class FirstNameRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string firstName)
        {
            var diff = FuzzyNameMatcher.LevenshteinDistance(patient.Name, firstName);
            if (diff == 0)
            {
                return new PatientWithRank<Patient>(patient, RankBuilder.StrongMatchRank,
                    MetaBuilder.FullMatchMeta(Match.FirstName));
            }

            return diff <= 2
                ? new PatientWithRank<Patient>(patient, new Rank(8), MetaBuilder.FullMatchMeta(Match.FirstName))
                : new PatientWithRank<Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}