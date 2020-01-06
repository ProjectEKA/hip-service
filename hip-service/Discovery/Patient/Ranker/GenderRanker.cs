using HipLibrary.Patient.Models.Response;

namespace hip_service.Discovery.Patient.Ranker
{
    public class GenderRanker: IRanker<Model.Patient>
    {
        public PatientWithRank<Model.Patient> Rank(Model.Patient patient, string gender)
        {
            return patient.Gender == gender
                ? new PatientWithRank<Model.Patient>(patient, RankBuilder.WeakMatchRank,
                    MetaBuilder.FullMatchMeta(Match.GENDER))
                : new PatientWithRank<Model.Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);

        }
    }
}