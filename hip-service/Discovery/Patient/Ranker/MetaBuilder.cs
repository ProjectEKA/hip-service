using HipLibrary.Patient.Models.Response;

namespace hip_service.Discovery.Patient.Ranker
{
    public static class MetaBuilder
    {
        private static Meta Meta(Match field, MatchLevel matchLevel) =>
            new Meta(field, matchLevel);

        public static Meta FullMatchMeta(Match field) =>
            Meta(field, MatchLevel.FullMatch);

        public static Meta EmptyMeta => new Meta(Match.EMPTY, MatchLevel.FullMatch);
    }
}