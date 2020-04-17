namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using static RankBuilder;
    using static MetaBuilder;

    public static class PatientWithRankBuilder
    {
        public static PatientWithRank<TPatient> EmptyRankWith<TPatient>(TPatient patient)
        {
            return new PatientWithRank<TPatient>(patient, EmptyRank, EmptyMeta);
        }
    }
}