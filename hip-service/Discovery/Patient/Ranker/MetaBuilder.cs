namespace hip_service.Discovery.Patient.Ranker
{
    public static class MetaBuilder
    {
        private static Meta Meta(string field, MatchLevel matchLevel) =>
            new Meta(field, matchLevel);

        public static Meta FullMatchMeta(string field) =>
            Meta(field, MatchLevel.FullMatch);

        public static Meta EmptyMeta => new Meta();
    }
}