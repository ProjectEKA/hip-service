namespace In.ProjectEKA.HipService.Discovery.Patient.Ranker
{
    using Model;

    public class EmptyRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string _)
        {
            return PatientWithRankBuilder.EmptyRankWith(patient);
        }
    }
}