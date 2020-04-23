namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using System;

    public enum MatchLevel
    {
        FullMatch
    }

    public struct Meta : IEquatable<Meta>
    {
        public string Field { get; }

        private readonly MatchLevel matchLevel;

        public Meta(string field, MatchLevel matchLevel)
        {
            Field = field;
            this.matchLevel = matchLevel;
        }

        public bool Equals(Meta other)
        {
            return matchLevel.Equals(other.matchLevel) && Field.Equals(other.Field);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) matchLevel * 397) ^ Field.GetHashCode();
            }
        }
    }
}