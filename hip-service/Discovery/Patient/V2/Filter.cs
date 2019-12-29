namespace hip_service.Discovery.Patient
{
    using System.Linq;
    using hip_library.Patient.models;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using static StrongMatcherFactory;
    using static hip_service.Discovery.Patient.RankBuilder;
    using static hip_service.Discovery.Patient.MetaBuilder;
    using hip_service.Discovery.Patient.models;

    public class Filter
    {
        readonly IMatchingRepository matchingRepository;

        public Filter(IMatchingRepository matchingRepository)
        {
            this.matchingRepository = matchingRepository;
        }

        private PatientWithRank<PatientInfo> RankPatient(PatientInfo patientInfo,
            DiscoveryRequest request)
        {
            static PatientWithRank<PatientInfo> rankMobile(PatientInfo patientInfo, string value)
            {
                return patientInfo.PhoneNumber == value ?
                    new PatientWithRank<PatientInfo>(patientInfo, StrongMatchRank, FullMatchMeta("MOBILE")) :
                    new PatientWithRank<PatientInfo>(patientInfo, EmptyRank, EmptyMeta);
            }

            static PatientWithRank<PatientInfo> rankName(PatientInfo patientInfo, string value)
            {
                return patientInfo.FirstName == value ?
                    new PatientWithRank<PatientInfo>(patientInfo, WeakMatchRank, FullMatchMeta("FirstName")) :
                    new PatientWithRank<PatientInfo>(patientInfo, EmptyRank, EmptyMeta);
            }
            return rankMobile(patientInfo,
                request.VerifiedIdentifiers.SingleOrDefault(
                    (Identifier arg) => arg.Type == IdentifierType.Mobile).Value)
                +
                rankName(patientInfo, request.FirstName);
        }

        public async Task<IQueryable<Patient>> DoFilter(DiscoveryRequest request)
        {
            var expression = GetExpression(request.VerifiedIdentifiers);
            return (await matchingRepository.Where(expression))
                    .AsEnumerable()
                    .Select(patientInfo => RankPatient(patientInfo, request))
                    .GroupBy(rankedPatient => rankedPatient.Rank.Score)
                    .OrderByDescending(rankedPatient => rankedPatient.Key)
                    .Take(1)
                    .SelectMany(group => {
                       return group.Select(rankedPatient => {
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
                       });
                    }).AsQueryable();
        }
    }

    internal class PatientWithRank<T>
    {
        public Rank Rank { get; }

        public ISet<Meta> Meta { get; }

        public T Patient { get; }

        public PatientWithRank(T patientInfo, Rank rank, Meta meta)
        {
            Patient = patientInfo;
            Rank = rank;
            Meta = new HashSet<Meta> { meta };
        }

        public PatientWithRank(T patientInfo, Rank rank, ISet<Meta> meta)
        {
            Patient = patientInfo;
            Rank = rank;
            Meta = meta;
        }

        public static PatientWithRank<T> operator +(PatientWithRank<T> left,
                                               PatientWithRank<T> right) =>
            new PatientWithRank<T>(left.Patient,
                Rank(left.Rank.Score + right.Rank.Score),
                left.Meta.Union(right.Meta).ToHashSet());
    }

    public struct Meta
    {
        public string Field { get; }

        public MatchLevel MatchLevel { get;  }

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
        public int Score { get;  }

        public Rank(int score)
        {
           Score = score;
        }
    }
}