namespace hip_service.Discovery.Patient.Ranker
{
    public class LastNameRanker : IRanker<models.Patient>
    {
        public PatientWithRank<models.Patient> Rank(models.Patient patient, string lastName)
        {
            return patient.LastName == lastName
                ? new PatientWithRank<models.Patient>(patient, RankBuilder.WeakMatchRank,
                    MetaBuilder.FullMatchMeta("LastName"))
                : new PatientWithRank<models.Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}