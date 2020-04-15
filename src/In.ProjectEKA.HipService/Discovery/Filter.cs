namespace In.ProjectEKA.HipService.Discovery
{
    using System.Collections.Generic;
    using System.Linq;
    using HipLibrary.Patient.Model;
    using Matcher;
    using Ranker;
    using static Ranker.PatientWithRankBuilder;
    using static HipLibrary.Matcher.StrongMatcherFactory;
    using static Ranker.RankBuilder;

    public static class Filter
    {
        private static readonly Dictionary<IdentifierType, IdentifierTypeExt> IdentifierTypeExts =
            new Dictionary<IdentifierType, IdentifierTypeExt>
            {
                {IdentifierType.MOBILE, IdentifierTypeExt.Mobile},
                {IdentifierType.MR, IdentifierTypeExt.Mr}
            };

        private static PatientWithRank<Patient> RankPatient(Patient patient, DiscoveryRequest request)
        {
            return RanksFor(request, patient)
                .Aggregate(EmptyRankWith(patient), (rank, withRank) => rank + withRank);
        }

        private static IEnumerable<PatientWithRank<Patient>> RanksFor(DiscoveryRequest request, Patient patient)
        {
            return From(request)
                .Select(identifier => Ranks
                    .GetValueOrDefault(identifier.Type, new EmptyRanker())
                    .Rank(patient, identifier.Value));
        }

        private static IEnumerable<IdentifierExt> From(DiscoveryRequest request)
        {
            var verifiedIdentifiers = request.Patient.VerifiedIdentifiers ?? new List<Identifier>();
            var unVerifiedIdentifiers = request.Patient.UnverifiedIdentifiers ?? new List<Identifier>();
            return verifiedIdentifiers
                .Select(identifier => new IdentifierExt(
                    IdentifierTypeExts.GetValueOrDefault(identifier.Type, IdentifierTypeExt.Empty),
                    identifier.Value))
                .Concat(unVerifiedIdentifiers
                    .Select(identifier => new IdentifierExt(
                        IdentifierTypeExts.GetValueOrDefault(identifier.Type, IdentifierTypeExt.Empty),
                        identifier.Value)))
                .Append(new IdentifierExt(IdentifierTypeExt.Name, request.Patient.Name))
                .Append(new IdentifierExt(IdentifierTypeExt.Gender, request.Patient.Gender.ToString()));
        }

        public static IEnumerable<PatientEnquiryRepresentation> Do(IEnumerable<Patient> patients,
            DiscoveryRequest request)
        {
            static bool IsMatching(string name1, string name2)
            {
                return FuzzyNameMatcher.LevenshteinDistance(name1, name2) <= 2;
            }

            var unverifiedExpression =
                GetUnVerifiedExpression(request.Patient.UnverifiedIdentifiers ?? new List<Identifier>());

            return patients
                .AsEnumerable()
                .Where(patient => patient.Gender == request.Patient.Gender)
                .Where(patient => IsMatching(patient.Name, request.Patient.Name))
                .Where(patient =>
                {
                    var ageGroupMatcher = new AgeGroupMatcher(2);
                    return !request.Patient.YearOfBirth.HasValue
                           || ageGroupMatcher.IsMatching(
                               AgeCalculator.From(request.Patient.YearOfBirth.Value),
                               AgeCalculator.From(patient.YearOfBirth));
                })
                .Where(unverifiedExpression.Compile())
                .Select(patientInfo => RankPatient(patientInfo, request))
                .GroupBy(rankedPatient => rankedPatient.Rank.Score)
                .OrderByDescending(rankedPatient => rankedPatient.Key)
                .Take(1)
                .SelectMany(group => group.Select(rankedPatient =>
                {
                    var careContexts = rankedPatient.Patient.CareContexts ?? new List<CareContextRepresentation>();

                    var careContextRepresentations = careContexts
                        .Select(program =>
                            new CareContextRepresentation(
                                program.ReferenceNumber,
                                program.Display))
                        .ToList();

                    return new PatientEnquiryRepresentation(
                        rankedPatient.Patient.Identifier,
                        $"{rankedPatient.Patient.Name}",
                        careContextRepresentations,
                        rankedPatient.Meta.Select(meta => meta.Field));
                }));
        }

        internal enum IdentifierTypeExt
        {
            Mobile,
            Name,
            Mr,
            Gender,
            Empty
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
    }
}