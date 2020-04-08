namespace In.ProjectEKA.HipService.Discovery
{
    using System.Collections.Generic;
    using System.Linq;
    using HipLibrary.Patient.Model;
    using Matcher;
    using Ranker;
    using static Ranker.PatientWithRankBuilder;

    public static class Filter
    {
        private static readonly Dictionary<IdentifierTypeExt, IRanker<Patient>> Ranks =
            new Dictionary<IdentifierTypeExt, IRanker<Patient>>
            {
                {IdentifierTypeExt.MOBILE, new MobileRanker()},
                {IdentifierTypeExt.FIRST_NAME, new FirstNameRanker()},
                {IdentifierTypeExt.LAST_NAME, new LastNameRanker()},
                {IdentifierTypeExt.GENDER, new GenderRanker()},
                {IdentifierTypeExt.EMPTY, new EmptyRanker()}
            };

        private static readonly Dictionary<IdentifierType, IdentifierTypeExt> IdentifierTypeExts =
            new Dictionary<IdentifierType, IdentifierTypeExt>
            {
                {IdentifierType.MOBILE, IdentifierTypeExt.MOBILE},
                {IdentifierType.MR, IdentifierTypeExt.MR}
            };

        private static PatientWithRank<Patient> RankPatient(Patient patient, DiscoveryRequest request)
        {
            return RanksFor(request, patient)
                .Aggregate(EmptyRankWith(patient),
                    (rank, withRank) => rank + withRank);
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
            return request.Patient.VerifiedIdentifiers
                .Select(identifier => new IdentifierExt(IdentifierTypeExts.GetValueOrDefault(identifier.Type,
                    IdentifierTypeExt.EMPTY), identifier.Value))
                .Concat(request.Patient.UnverifiedIdentifiers
                    .Select(identifier => new IdentifierExt(IdentifierTypeExts.GetValueOrDefault(identifier.Type,
                        IdentifierTypeExt.EMPTY), identifier.Value)))
                .Append(new IdentifierExt(IdentifierTypeExt.FIRST_NAME, request.Patient.FirstName))
                .Append(new IdentifierExt(IdentifierTypeExt.LAST_NAME, request.Patient.LastName))
                .Append(new IdentifierExt(IdentifierTypeExt.GENDER, request.Patient.Gender.ToString()));
        }

        public static IEnumerable<PatientEnquiryRepresentation> Do(IEnumerable<Patient> patients,
            DiscoveryRequest request)
        {
            static bool IsMatching(string name1, string name2)
            {
                return FuzzyNameMatcher.LevenshteinDistance(name1, name2) <= 2;
            }

            return patients
                .AsEnumerable()
                .Where(patient => patient.Gender == request.Patient.Gender)
                .Where(patient => IsMatching(patient.FirstName, request.Patient.FirstName))
                .Select(patientInfo => RankPatient(patientInfo, request))
                .GroupBy(rankedPatient => rankedPatient.Rank.Score)
                .OrderByDescending(rankedPatient => rankedPatient.Key)
                .Take(1)
                .SelectMany(group => group.Select(rankedPatient =>
                {
                    var isCareContextPresent = !(rankedPatient.Patient.CareContexts == null ||
                                                 !rankedPatient.Patient.CareContexts.Any());

                    var careContexts = isCareContextPresent
                        ? rankedPatient.Patient.CareContexts
                            .Select(program =>
                                new CareContextRepresentation(
                                    program.ReferenceNumber,
                                    program.Display))
                            .ToList()
                        : new List<CareContextRepresentation>();

                    return new PatientEnquiryRepresentation(
                        rankedPatient.Patient.Identifier,
                        $"{rankedPatient.Patient.FirstName} {rankedPatient.Patient.LastName}",
                        careContexts, rankedPatient.Meta.Select(meta => meta.Field));
                }));
        }

        private enum IdentifierTypeExt
        {
            MOBILE,
            FIRST_NAME,
            LAST_NAME,
            MR,
            GENDER,
            EMPTY
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