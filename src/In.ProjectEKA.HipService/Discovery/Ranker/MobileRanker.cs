namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model;
    using static RankBuilder;
    using static MetaBuilder;

    public class MobileRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string mobile)
        {
            return patient.PhoneNumber == mobile
                ? new PatientWithRank<Patient>(patient, StrongMatchRank, FullMatchMeta(Match.Mobile))
                : new PatientWithRank<Patient>(patient, EmptyRank, EmptyMeta);
        }
    }
}