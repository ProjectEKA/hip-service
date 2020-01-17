namespace In.ProjectEKA.HipService.Discovery.Patient.Ranker
{
    public static class PatientWithRankBuilder
    {
        public static PatientWithRank<TPatient> EmptyRankWith<TPatient>(TPatient patient)
        {
            return new PatientWithRank<TPatient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}