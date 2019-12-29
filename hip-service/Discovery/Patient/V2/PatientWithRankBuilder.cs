namespace hip_service.Discovery.Patient
{
    internal static class PatientWithRankBuilder
    {
        public static PatientWithRank<TPatient> EmptyRankWith<TPatient>(TPatient patient)
        {
            return  new PatientWithRank<TPatient>(patient, RankBuilder.EmptyRank, MetaBuilder.EmptyMeta);
        }
    }
}