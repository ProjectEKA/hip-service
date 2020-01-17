namespace In.ProjectEKA.HipService.Discovery.Patient.Ranker
{
    using System;

    public struct Meta : IEquatable<Meta>
    {
        public string Field { get; }

        private readonly MatchLevel MatchLevel;

        public Meta(string field, MatchLevel matchLevel)
        {
            Field = field;
            MatchLevel = matchLevel;
        }

        public bool Equals(Meta other)
        {
            return MatchLevel.Equals(other.MatchLevel) && Field.Equals(other.Field);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) MatchLevel * 397) ^ Field.GetHashCode();
            }
        }
    }
}