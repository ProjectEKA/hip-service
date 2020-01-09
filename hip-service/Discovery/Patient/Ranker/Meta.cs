using System;
using HipLibrary.Patient.Model.Response;

namespace hip_service.Discovery.Patient.Ranker
{
    public struct Meta: IEquatable<Meta>
    {
        public Match Field { get; }

        private readonly MatchLevel MatchLevel;

        public Meta(Match field, MatchLevel matchLevel)
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
                return ((int) MatchLevel * 397) ^ (Field.GetHashCode());
            }
        }
    }
}