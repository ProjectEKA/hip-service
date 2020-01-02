namespace hip_service.Discovery.Patient.Ranker
{
    public struct Rank
    {
        public int Score { get; }

        public Rank(int score)
        {
            Score = score;
        }
    }
}