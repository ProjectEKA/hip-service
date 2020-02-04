namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model;
    using Matcher;
    using Patient = HipLibrary.Patient.Model.Patient;

    public class FirstNameRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string firstName)
        {
            var diff = FuzzyNameMatcher.LevenshteinDistance(patient.FirstName, firstName);
            if (diff == 0)
                return new PatientWithRank<Patient>(patient, RankBuilder.StrongMatchRank,
                    MetaBuilder.FullMatchMeta(Match.FirstName));
            if (diff <= 2)
                return new PatientWithRank<Patient>(patient, new Rank(8), MetaBuilder.FullMatchMeta(Match.FirstName));

            return new PatientWithRank<Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}