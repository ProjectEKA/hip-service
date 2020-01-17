namespace In.ProjectEKA.HipService.Discovery.Patient.Ranker
{
    using HipLibrary.Patient.Model.Response;

    public static class MetaBuilder
    {
        public static Meta EmptyMeta => new Meta(Match.EMPTY.ToString(), MatchLevel.FullMatch);

        private static Meta Meta(string field, MatchLevel matchLevel)
        {
            return new Meta(field, matchLevel);
        }

        public static Meta FullMatchMeta(Match field)
        {
            return Meta(field.ToString(), MatchLevel.FullMatch);
        }
    }
}