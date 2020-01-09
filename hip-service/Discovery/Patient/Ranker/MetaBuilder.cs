using HipLibrary.Patient.Model.Response;

namespace hip_service.Discovery.Patient.Ranker
{
    public static class MetaBuilder
    {
        private static Meta Meta(string field, MatchLevel matchLevel) =>
            new Meta(field, matchLevel);

        public static Meta FullMatchMeta(Match field) =>
            Meta(field.ToString(), MatchLevel.FullMatch);

        public static Meta EmptyMeta => new Meta(Match.EMPTY.ToString(), MatchLevel.FullMatch);
    }
}