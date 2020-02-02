namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using HipLibrary.Patient.Model.Response;
    using Patient = Model.Patient;

    public class MobileRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string mobile)
        {
            return patient.PhoneNumber == mobile
                ? new PatientWithRank<Patient>(patient, RankBuilder.StrongMatchRank,
                    MetaBuilder.FullMatchMeta(Match.MOBILE))
                : new PatientWithRank<Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}