using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using hip_library.Patient.models;
using hip_service.Discovery.Patient.Helper;

namespace hip_service.Discovery.Patient.Matcher
{
    public static class StrongMatcherFactory
    {
        private static readonly Dictionary<IdentifierType, IIdentifierMatcher> Matchers =
        new Dictionary<IdentifierType, IIdentifierMatcher>
        {
            { IdentifierType.Mobile, new PhoneNumberMatcher() }
        };

        private static Expression<Func<models.Patient, bool>> ToExpression(Identifier identifier) =>
            Matchers.GetValueOrDefault(identifier.Type, new EmptyMatcher()).Of(identifier.Value);
        public static Expression<Func<models.Patient, bool>> GetExpression(IEnumerable<Identifier> identifiers) =>
            identifiers
                .Select(ToExpression)
                .Aggregate(ExpressionBuilder.False<models.Patient>(),
                    (accumulate, next) => accumulate.Or(next));
    }
}