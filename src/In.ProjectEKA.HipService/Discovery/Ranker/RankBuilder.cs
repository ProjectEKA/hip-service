namespace In.ProjectEKA.HipService.Discovery.Ranker
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;

    public class RankBuilder
    {
        private RankBuilder()
        {
        }

        public static Rank EmptyRank => Rank(0);

        public static Rank StrongMatchRank => Rank(10);

        public static Rank WeakMatchRank => Rank(1);

        public static Rank Rank(int score)
        {
            return new Rank(score);
        }

        internal static readonly Dictionary<Filter.IdentifierTypeExt, IRanker<Patient>> Ranks =
            new Dictionary<Filter.IdentifierTypeExt, IRanker<Patient>>
            {
                {Filter.IdentifierTypeExt.Mobile, new MobileRanker()},
                {Filter.IdentifierTypeExt.Name, new NameRanker()},
                {Filter.IdentifierTypeExt.Gender, new GenderRanker()},
                {Filter.IdentifierTypeExt.Mr, new MedicalRecordRanker()},
                {Filter.IdentifierTypeExt.Empty, new EmptyRanker()}
            };
    }
}