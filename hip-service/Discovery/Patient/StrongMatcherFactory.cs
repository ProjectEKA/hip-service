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
        private static Dictionary<IdentifierType, IIdentifierMatcher> matchers =
        new Dictionary<IdentifierType, IIdentifierMatcher>() {
            { IdentifierType.Mobile, new PhoneNumberMatcher() }
        };

        private static Expression<Func<PatientInfo, bool>> toExpression(Identifier identifier) =>
            matchers.GetValueOrDefault(identifier.Type, new EmptyMatcher()).of(identifier.Value);
        public static Expression<Func<PatientInfo, bool>> GetExpression(IEnumerable<Identifier> identifiers) =>
            identifiers
                .Select(toExpression)
                .Aggregate(ExpressionBuilder.False<PatientInfo>(), (accumalate, next) => accumalate.Or(next));
    }
}