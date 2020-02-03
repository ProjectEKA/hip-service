namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model;

    public class EmptyRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string _)
        {
            return PatientWithRankBuilder.EmptyRankWith(patient);
        }
    }
}