using HipLibrary.Patient.Model.Response;

namespace hip_service.Discovery.Patient.Ranker
{
    public class FirstNameRanker : IRanker<Model.Patient>
    {
        public PatientWithRank<Model.Patient> Rank(Model.Patient patient, string firstName)
        {
            var diff = Helpers.FuzzyNameMatcher.LevenshteinDistance(patient.FirstName, firstName);
            if (diff == 0) 
            {
                return new PatientWithRank<Model.Patient>(patient, RankBuilder.StrongMatchRank, MetaBuilder.FullMatchMeta(Match.FIRST_NAME));
            }
            if (diff <= 2) 
            {
                return new PatientWithRank<Model.Patient>(patient, new Rank(8), MetaBuilder.FullMatchMeta(Match.FIRST_NAME));
            }

            return new PatientWithRank<Model.Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}