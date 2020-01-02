namespace hip_service.Discovery.Patient.Ranker
{
    public struct Meta
    {
        public string Field { get; }

        private MatchLevel MatchLevel;

        public Meta(string field, MatchLevel matchLevel)
        {
            Field = field;
            MatchLevel = matchLevel;
        }
    }
}