namespace hip_service.Discovery.Patient.Ranker
{
    public class GenderRanker: IRanker<models.Patient>
    {
        public PatientWithRank<models.Patient> Rank(models.Patient patient, string gender)
        {
            return patient.Gender == gender
                ? new PatientWithRank<models.Patient>(patient, RankBuilder.WeakMatchRank,
                    MetaBuilder.FullMatchMeta("Gender"))
                : new PatientWithRank<models.Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);

        }
    }
}