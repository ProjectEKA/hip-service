namespace hip_service.Discovery.Patient.Ranker
{
    public interface IRanker<T>
    {
        PatientWithRank<T> Rank(T element, string by);
    }
}