using System;

namespace hip_service.Discovery.Patient.Ranker
{
    public struct Rank: IEquatable<Rank>
    {
        public int Score { get; }

        public Rank(int score)
        {
            Score = score;
        }

        public bool Equals(Rank other)
        {
            return Score == other.Score;
        }

        public override bool Equals(object obj)
        {
            return obj is Rank other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Score;
        }
    }
}