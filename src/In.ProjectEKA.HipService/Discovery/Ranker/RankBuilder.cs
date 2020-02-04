namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    public static class RankBuilder
    {
        public static Rank EmptyRank => Rank(0);

        public static Rank StrongMatchRank => Rank(10);

        public static Rank WeakMatchRank => Rank(1);

        public static Rank Rank(int score)
        {
            return new Rank(score);
        }
    }
}