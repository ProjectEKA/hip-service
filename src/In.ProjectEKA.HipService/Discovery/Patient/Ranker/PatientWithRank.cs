namespace In.ProjectEKA.HipService.Discovery.Patient.Ranker
{
    using System.Collections.Generic;
    using System.Linq;

    public class PatientWithRank<T>
    {
        public PatientWithRank(T patient, Rank rank, Meta meta)
        {
            Patient = patient;
            Rank = rank;
            Meta = new HashSet<Meta> {meta};
        }

        private PatientWithRank(T patient, Rank rank, ISet<Meta> meta)
        {
            Patient = patient;
            Rank = rank;
            Meta = meta;
        }

        public Rank Rank { get; }

        public ISet<Meta> Meta { get; }

        public T Patient { get; }

        public static PatientWithRank<T> operator +(PatientWithRank<T> left,
            PatientWithRank<T> right)
        {
            return new PatientWithRank<T>(left.Patient,
                RankBuilder.Rank(left.Rank.Score + right.Rank.Score),
                left.Meta.Union(right.Meta).Where(meta => !meta.Equals(MetaBuilder.EmptyMeta)).ToHashSet());
        }
    }
}