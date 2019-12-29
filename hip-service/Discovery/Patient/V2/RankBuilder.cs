namespace hip_service.Discovery.Patient
{
    public static class RankBuilder
    {
        public static Rank Rank(int score) => new Rank(score);

        public static Rank EmptyRank => Rank(0);
        
        public static Rank StrongMatchRank =>  Rank(10);

        public static Rank WeakMatchRank => Rank(1);
    }
}