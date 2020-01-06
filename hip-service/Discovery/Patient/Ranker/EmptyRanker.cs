namespace hip_service.Discovery.Patient.Ranker
{
    public class EmptyRanker : IRanker<models.Patient>
    {
        public PatientWithRank<models.Patient> Rank(models.Patient patient, string _)
        {
            return PatientWithRankBuilder.EmptyRankWith(patient);
        }
    }
}