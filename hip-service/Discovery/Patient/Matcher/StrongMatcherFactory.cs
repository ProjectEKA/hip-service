using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using hip_service.Discovery.Patient.Helper;
using HipLibrary.Patient.Models;

namespace hip_service.Discovery.Patient.Matcher
{
    public static class StrongMatcherFactory
    {
        private static readonly Dictionary<IdentifierType, IIdentifierMatcher> Matchers =
        new Dictionary<IdentifierType, IIdentifierMatcher>
        {
            { IdentifierType.MOBILE, new PhoneNumberMatcher() }
        };

        private static Expression<Func<Model.Patient, bool>> ToExpression(Identifier identifier) =>
            Matchers.GetValueOrDefault(identifier.Type, new EmptyMatcher()).Of(identifier.Value);
        public static Expression<Func<Model.Patient, bool>> GetExpression(IEnumerable<Identifier> identifiers) =>
            identifiers
                .Select(ToExpression)
                .Aggregate(ExpressionBuilder.False<Model.Patient>(),
                    (accumulate, next) => accumulate.Or(next));
    }
}