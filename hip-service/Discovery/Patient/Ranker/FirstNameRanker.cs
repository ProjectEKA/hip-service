namespace hip_service.Discovery.Patient.Ranker
{
    public class FirstNameRanker : IRanker<models.Patient>
    {
        public PatientWithRank<models.Patient> Rank(models.Patient patient, string firstName)
        {
            return patient.FirstName == firstName
                ? new PatientWithRank<models.Patient>(patient, RankBuilder.WeakMatchRank,
                    MetaBuilder.FullMatchMeta("FirstName"))
                : new PatientWithRank<models.Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}