namespace hip_service.Discovery.Patient
{
    using static RankBuilder;
    using static PatientWithRankBuilder;
    using static MetaBuilder;
    using System.Collections.Generic;
    using System.Linq;
    using hip_library.Patient.models;
    using models;

    public class Filter
    {
        private enum IdentifierTypeExt
        {
            Mobile,
            FirstName,
            LastName,
            Mr,
            Empty
        }

        private PatientWithRank<PatientInfo> RankPatient(PatientInfo patientInfo,
            DiscoveryRequest request)
        {
            return RanksFor(request, patientInfo)
                .Aggregate(EmptyRankWith(patientInfo),
                    (rank, withRank) => rank + withRank);
        }

        private IEnumerable<PatientWithRank<PatientInfo>> RanksFor(DiscoveryRequest request, PatientInfo patientInfo)
        {
            var ranks = new Dictionary<IdentifierTypeExt, IRanker<PatientInfo>>
            {
                {IdentifierTypeExt.Mobile, new MobileRanker()},
                {IdentifierTypeExt.FirstName, new FirstNameRanker()},
                {IdentifierTypeExt.LastName, new LastNameRanker()},
                {IdentifierTypeExt.Empty, new EmptyRanker()}
            };
            return From(request).Select(identifier =>
                ranks.GetValueOrDefault(identifier.Type, new EmptyRanker())
                    .Rank(patientInfo, identifier.Value));
        }

        private static IEnumerable<IdentifierExt> From(DiscoveryRequest request)
        {
            var identifierTypeExts = new Dictionary<IdentifierType, IdentifierTypeExt>
            {
                {IdentifierType.Mobile, IdentifierTypeExt.Mobile},
                {IdentifierType.Mr, IdentifierTypeExt.Mr}
            };
            return request.VerifiedIdentifiers
                .Select(identifier => new IdentifierExt(identifierTypeExts.GetValueOrDefault(identifier.Type,
                    IdentifierTypeExt.Empty), identifier.Value))
                .Concat(request.UnverifiedIdentifiers
                    .Select(identifier => new IdentifierExt(identifierTypeExts.GetValueOrDefault(identifier.Type,
                        IdentifierTypeExt.Empty), identifier.Value)))
                .Append(new IdentifierExt(IdentifierTypeExt.FirstName, request.FirstName))
                .Append(new IdentifierExt(IdentifierTypeExt.LastName, request.LastName));
        }

        public IEnumerable<Patient> Do(IEnumerable<PatientInfo> patients, DiscoveryRequest request)
        {
            return patients
                .AsEnumerable()
                .Select(patientInfo => RankPatient(patientInfo, request))
                .GroupBy(rankedPatient => rankedPatient.Rank.Score)
                .OrderByDescending(rankedPatient => rankedPatient.Key)
                .Take(1)
                .SelectMany(group => group.Select(rankedPatient =>
                {
                    var careContexts = rankedPatient.Patient.Programs
                        .Select(program =>
                            new CareContextRepresentation(
                                program.ReferenceNumber,
                                program.Description))
                        .ToList();

                    return new Patient(
                        rankedPatient.Patient.Identifier,
                        $"{rankedPatient.Patient.FirstName} {rankedPatient.Patient.LastName}",
                        careContexts, rankedPatient.Meta.Select(meta => meta.Field));
                }));
        }

        private class IdentifierExt
        {
            public IdentifierExt(IdentifierTypeExt type, string value)
            {
                Type = type;
                Value = value;
            }

            public IdentifierTypeExt Type { get; }
            public string Value { get; }
        }

        private interface IRanker<T>
        {
            PatientWithRank<T> Rank(T element, string by);
        }

        private class FirstNameRanker : IRanker<PatientInfo>
        {
            public PatientWithRank<PatientInfo> Rank(PatientInfo patientInfo, string firstName)
            {
                return patientInfo.FirstName == firstName
                    ? new PatientWithRank<PatientInfo>(patientInfo, WeakMatchRank, FullMatchMeta("FirstName"))
                    : new PatientWithRank<PatientInfo>(patientInfo, EmptyRank, EmptyMeta);
            }
        }

        private class MobileRanker : IRanker<PatientInfo>
        {
            public PatientWithRank<PatientInfo> Rank(PatientInfo patientInfo, string mobile)
            {
                return patientInfo.PhoneNumber == mobile
                    ? new PatientWithRank<PatientInfo>(patientInfo, StrongMatchRank, FullMatchMeta("MOBILE"))
                    : new PatientWithRank<PatientInfo>(patientInfo, EmptyRank, EmptyMeta);
            }
        }

        private class EmptyRanker : IRanker<PatientInfo>
        {
            public PatientWithRank<PatientInfo> Rank(PatientInfo patientInfo, string _)
            {
                return EmptyRankWith(patientInfo);
            }
        }

        private class LastNameRanker : IRanker<PatientInfo>
        {
            public PatientWithRank<PatientInfo> Rank(PatientInfo patientInfo, string lastName)
            {
                return patientInfo.LastName == lastName
                    ? new PatientWithRank<PatientInfo>(patientInfo, WeakMatchRank, FullMatchMeta("LastName"))
                    : new PatientWithRank<PatientInfo>(patientInfo, EmptyRank, EmptyMeta);
            }
        }
    }

    internal class PatientWithRank<T>
    {
        public PatientWithRank(T patientInfo, Rank rank, Meta meta)
        {
            Patient = patientInfo;
            Rank = rank;
            Meta = new HashSet<Meta> {meta};
        }

        private PatientWithRank(T patientInfo, Rank rank, ISet<Meta> meta)
        {
            Patient = patientInfo;
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
                Rank(left.Rank.Score + right.Rank.Score),
                left.Meta.Union(right.Meta).Where(meta => !meta.Equals(EmptyMeta)).ToHashSet());
        }
    }

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

    public enum MatchLevel
    {
        FullMatch
    }

    public struct Rank
    {
        public int Score { get; }

        public Rank(int score)
        {
            Score = score;
        }
    }
}