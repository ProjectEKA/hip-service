namespace In.ProjectEKA.HipService.Discovery.Patient.Ranker
{
    using HipLibrary.Patient.Model.Response;
    using Matcher;
    using Patient = Model.Patient;

    public class FirstNameRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string firstName)
        {
            var diff = FuzzyNameMatcher.LevenshteinDistance(patient.FirstName, firstName);
            if (diff == 0)
                return new PatientWithRank<Patient>(patient, RankBuilder.StrongMatchRank,
                    MetaBuilder.FullMatchMeta(Match.FIRST_NAME));
            if (diff <= 2)
                return new PatientWithRank<Patient>(patient, new Rank(8), MetaBuilder.FullMatchMeta(Match.FIRST_NAME));

            return new PatientWithRank<Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}