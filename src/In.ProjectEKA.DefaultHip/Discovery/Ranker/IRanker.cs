namespace In.ProjectEKA.DefaultHip.Discovery.Ranker
{
    public interface IRanker<T>
    {
        PatientWithRank<T> Rank(T element, string by);
    }
}