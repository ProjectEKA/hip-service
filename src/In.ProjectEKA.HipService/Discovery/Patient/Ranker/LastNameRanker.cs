namespace In.ProjectEKA.HipService.Discovery.Patient.Ranker
{
    using Helper;
    using HipLibrary.Patient.Model.Response;
    using Matcher;
    using Patient = Model.Patient;

    public class LastNameRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string lastName)
        {
            var diff = FuzzyNameMatcher.LevenshteinDistance(patient.LastName, lastName);
            if (diff == 0)
                return new PatientWithRank<Patient>(patient, RankBuilder.StrongMatchRank,
                    MetaBuilder.FullMatchMeta(Match.LAST_NAME));
            if (diff <= 2)
                return new PatientWithRank<Patient>(patient, new Rank(5), MetaBuilder.FullMatchMeta(Match.LAST_NAME));

            return new PatientWithRank<Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}