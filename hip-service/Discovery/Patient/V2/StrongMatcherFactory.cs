namespace hip_service.Discovery.Patient
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using hip_service.Discovery.Patient.models;
    using hip_library.Patient.models;

    public static class StrongMatcherFactory
    {
        private static readonly Dictionary<IdentifierType, IIdentifierMatcher> Matchers =
        new Dictionary<IdentifierType, IIdentifierMatcher>() {
            { IdentifierType.Mobile, new PhoneNumberMatcher() }
        };

        private static Expression<Func<PatientInfo, bool>> ToExpression(Identifier identifier) =>
            Matchers.GetValueOrDefault(identifier.Type, new EmptyMatcher()).of(identifier.Value);
        public static Expression<Func<PatientInfo, bool>> GetExpression(IEnumerable<Identifier> identifiers) =>
            identifiers
                .Select(ToExpression)
                .Aggregate(ExpressionBuilder.False<PatientInfo>(),
                    (accumulate, next) => accumulate.Or(next));
    }
}