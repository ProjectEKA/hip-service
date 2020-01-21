namespace In.ProjectEKA.DefaultHip.Discovery.Matcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Helper;
    using HipLibrary.Patient.Model;
    using Model;

    public static class StrongMatcherFactory
    {
        private static readonly Dictionary<IdentifierType, IIdentifierMatcher> Matchers =
            new Dictionary<IdentifierType, IIdentifierMatcher>
            {
                {IdentifierType.MOBILE, new PhoneNumberMatcher()}
            };

        private static Expression<Func<Patient, bool>> ToExpression(Identifier identifier)
        {
            return Matchers.GetValueOrDefault(identifier.Type, new EmptyMatcher()).Of(identifier.Value);
        }

        public static Expression<Func<Patient, bool>> GetExpression(IEnumerable<Identifier> identifiers)
        {
            return identifiers
                .Select(ToExpression)
                .Aggregate(ExpressionBuilder.False<Patient>(),
                    (accumulate, next) => accumulate.Or(next));
        }
    }
}