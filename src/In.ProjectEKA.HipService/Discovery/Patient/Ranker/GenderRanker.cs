namespace In.ProjectEKA.HipService.Discovery.Patient.Ranker
{
    using HipLibrary.Patient.Model.Response;
    using Patient = Model.Patient;

    public class GenderRanker : IRanker<Patient>
    {
        public PatientWithRank<Patient> Rank(Patient patient, string gender)
        {
            return patient.Gender == gender
                ? new PatientWithRank<Patient>(patient, RankBuilder.WeakMatchRank,
                    MetaBuilder.FullMatchMeta(Match.GENDER))
                : new PatientWithRank<Patient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}