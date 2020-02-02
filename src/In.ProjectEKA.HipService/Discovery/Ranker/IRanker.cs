namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    public interface IRanker<T>
    {
        PatientWithRank<T> Rank(T element, string by);
    }
}