using System.Collections.Generic;
using System.Linq;
using hip_library.Patient.models;
using hip_service.Discovery.Patient.Ranker;

namespace hip_service.Discovery.Patient
{
    using static PatientWithRankBuilder;

    public class Filter
    {
        private enum IdentifierTypeExt
        {
            Mobile,
            FirstName,
            LastName,
            Mr,
            Gender,
            Empty
        }

        private PatientWithRank<models.Patient> RankPatient(models.Patient patient,
            DiscoveryRequest request)
        {
            return RanksFor(request, patient)
                .Aggregate(EmptyRankWith(patient),
                    (rank, withRank) => rank + withRank);
        }

        private static IEnumerable<PatientWithRank<models.Patient>> RanksFor(DiscoveryRequest request, models.Patient patient)
        {
            var ranks = new Dictionary<IdentifierTypeExt, IRanker<models.Patient>>
            {
                {IdentifierTypeExt.Mobile, new MobileRanker()},
                {IdentifierTypeExt.FirstName, new FirstNameRanker()},
                {IdentifierTypeExt.LastName, new LastNameRanker()},
                {IdentifierTypeExt.Gender, new GenderRanker()},
                {IdentifierTypeExt.Empty, new EmptyRanker()}
            };
            return From(request).Select(identifier =>
                ranks.GetValueOrDefault(identifier.Type, new EmptyRanker())
                    .Rank(patient, identifier.Value));
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
                .Append(new IdentifierExt(IdentifierTypeExt.LastName, request.LastName))
                .Append(new IdentifierExt(IdentifierTypeExt.Gender, request.Gender.ToString()));
        }

        public IEnumerable<hip_library.Patient.models.Patient> Do(IEnumerable<models.Patient> patients,
            DiscoveryRequest request)
        {
            return patients
                .AsEnumerable()
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
                                    program.Description))
                            .ToList()
                        : new List<CareContextRepresentation>();

                    return new hip_library.Patient.models.Patient(
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
    }

    public enum MatchLevel
    {
        FullMatch
    }
}