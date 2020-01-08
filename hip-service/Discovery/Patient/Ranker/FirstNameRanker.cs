using HipLibrary.Patient.Model.Response;

namespace hip_service.Discovery.Patient.Ranker
{
    public class FirstNameRanker : IRanker<Model.Patient>
    {
        public PatientWithRank<Model.Patient> Rank(Model.Patient patient, string firstName)
        {
            return patient.FirstName == firstName
                ? new PatientWithRank<Model.Patient>(patient, RankBuilder.WeakMatchRank,
                    MetaBuilder.FullMatchMeta(Match.FIRST_NAME))
                : new PatientWithRank<Model.Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}