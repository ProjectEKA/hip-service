using HipLibrary.Patient.Model.Response;

namespace hip_service.Discovery.Patient.Ranker
{
    public class LastNameRanker : IRanker<Model.Patient>
    {
        public PatientWithRank<Model.Patient> Rank(Model.Patient patient, string lastName)
        {
            return patient.LastName == lastName
                ? new PatientWithRank<Model.Patient>(patient, RankBuilder.WeakMatchRank,
                    MetaBuilder.FullMatchMeta(Match.LAST_NAME))
                : new PatientWithRank<Model.Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}