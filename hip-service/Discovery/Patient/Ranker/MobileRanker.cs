namespace hip_service.Discovery.Patient.Ranker
{
    public class MobileRanker : IRanker<models.Patient>
    {
        public PatientWithRank<models.Patient> Rank(models.Patient patient, string mobile)
        {
            return patient.PhoneNumber == mobile
                ? new PatientWithRank<models.Patient>(patient, RankBuilder.StrongMatchRank,
                    MetaBuilder.FullMatchMeta("MOBILE"))
                : new PatientWithRank<models.Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}