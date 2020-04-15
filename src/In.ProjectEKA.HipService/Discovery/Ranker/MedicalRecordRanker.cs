namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model;
    using static RankBuilder;
    using static MetaBuilder;

    internal class MedicalRecordRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string referenceNumber)
        {
            return patient.Identifier == referenceNumber
                ? new PatientWithRank<Patient>(patient, StrongMatchRank, FullMatchMeta(Match.Mr))
                : new PatientWithRank<Patient>(patient, EmptyRank, EmptyMeta);
        }
    }
}