using System.Collections.Generic;
using System.Linq;
using HipLibrary.Patient.Model;
using hip_service.Discovery.Patient.Ranker;
using HipLibrary.Patient.Model.Request;

namespace hip_service.Discovery.Patient
{
    using static PatientWithRankBuilder;

    public class Filter
    {
        private enum IdentifierTypeExt
        {
            MOBILE,
            FIRST_NAME,
            LAST_NAME,
            MR,
            GENDER,
            EMPTY
        }

        private PatientWithRank<Model.Patient> RankPatient(Model.Patient patient,
            DiscoveryRequest request)
        {
            return RanksFor(request, patient)
                .Aggregate(EmptyRankWith(patient),
                    (rank, withRank) => rank + withRank);
        }
        private static readonly Dictionary<IdentifierTypeExt, IRanker<Model.Patient>> Ranks = new Dictionary<IdentifierTypeExt, IRanker<Model.Patient>>
        {
            {IdentifierTypeExt.MOBILE, new MobileRanker()},
            {IdentifierTypeExt.FIRST_NAME, new FirstNameRanker()},
            {IdentifierTypeExt.LAST_NAME, new LastNameRanker()},
            {IdentifierTypeExt.GENDER, new GenderRanker()},
            {IdentifierTypeExt.EMPTY, new EmptyRanker()}
        };

        private static readonly Dictionary<IdentifierType, IdentifierTypeExt> IdentifierTypeExts = new Dictionary<IdentifierType, IdentifierTypeExt>
        {
            {IdentifierType.MOBILE, IdentifierTypeExt.MOBILE},
            {IdentifierType.MR, IdentifierTypeExt.MR}
        };

        private static IEnumerable<PatientWithRank<Model.Patient>> RanksFor(DiscoveryRequest request, Model.Patient patient)
        {
            return From(request).Select(identifier =>
                Ranks.GetValueOrDefault(identifier.Type, new EmptyRanker())
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

        public IEnumerable<HipLibrary.Patient.Model.Response.Patient> Do(IEnumerable<Model.Patient> patients,
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

                    return new HipLibrary.Patient.Model.Response.Patient(
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