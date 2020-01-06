using HipLibrary.Patient.Models.Response;

namespace hip_service.Discovery.Patient.Ranker
{
    public class MobileRanker : IRanker<Model.Patient>
    {
        public PatientWithRank<Model.Patient> Rank(Model.Patient patient, string mobile)
        {
            return patient.PhoneNumber == mobile
                ? new PatientWithRank<Model.Patient>(patient, RankBuilder.StrongMatchRank,
                    MetaBuilder.FullMatchMeta(Match.MOBILE))
                : new PatientWithRank<Model.Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}