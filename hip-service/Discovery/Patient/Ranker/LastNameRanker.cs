using HipLibrary.Patient.Model.Response;

namespace hip_service.Discovery.Patient.Ranker
{
    public class LastNameRanker : IRanker<Model.Patient>
    {
        public PatientWithRank<Model.Patient> Rank(Model.Patient patient, string lastName)
        {
            
            var diff = Helpers.FuzzyNameMatcher.LevenshteinDistance(patient.LastName, lastName);
            if (diff == 0) 
            {
                return new PatientWithRank<Model.Patient>(patient, RankBuilder.StrongMatchRank, MetaBuilder.FullMatchMeta(Match.LAST_NAME));
            }
            if (diff <= 2) 
            {
                return new PatientWithRank<Model.Patient>(patient, new Rank(5), MetaBuilder.FullMatchMeta(Match.LAST_NAME));
            }

            return new PatientWithRank<Model.Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
            
        }
    }
}