namespace hip_service.Discovery.Patient.Ranker
{
    public class EmptyRanker : IRanker<Model.Patient>
    {
        public PatientWithRank<Model.Patient> Rank(Model.Patient patient, string _)
        {
            return PatientWithRankBuilder.EmptyRankWith(patient);
        }
    }
}