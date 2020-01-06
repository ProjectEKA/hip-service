using System;

namespace hip_service.Discovery.Patient.Ranker
{
    public struct Meta: IEquatable<Meta>
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
            return MatchLevel == other.MatchLevel && string.Equals(Field, other.Field);
        }

        public override bool Equals(object obj)
        {
            return obj is Meta other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) MatchLevel * 397) ^ (Field != null ? Field.GetHashCode() : 0);
            }
        }
    }
}