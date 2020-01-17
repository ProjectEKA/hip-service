namespace In.ProjectEKA.HipService.Discovery.Patient.Ranker
{
    public interface IRanker<T>
    {
        PatientWithRank<T> Rank(T element, string by);
    }
}