namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model;
    using Matcher;

    public class LastNameRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string lastName)
        {
            var diff = FuzzyNameMatcher.LevenshteinDistance(patient.LastName, lastName);
            if (diff == 0)
                return new PatientWithRank<Patient>(patient, RankBuilder.StrongMatchRank,
                    MetaBuilder.FullMatchMeta(Match.LastName));
            if (diff <= 2)
                return new PatientWithRank<Patient>(patient, new Rank(5), MetaBuilder.FullMatchMeta(Match.LastName));

            return new PatientWithRank<Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}