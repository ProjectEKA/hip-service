namespace In.ProjectEKA.HipLibrary.Matcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Optional.Collections;
    using Patient.Model;

    public class StrongMatcherFactory
    {
        private static readonly Dictionary<IdentifierType, IIdentifierMatcher> Matchers =
            new Dictionary<IdentifierType, IIdentifierMatcher>
            {
                {IdentifierType.MOBILE, new PhoneNumberMatcher()},
                {IdentifierType.MR, new MedicalRecordMatcher()}
            };

        private StrongMatcherFactory()
        {
        }

        private static Expression<Func<Patient, bool>> ToExpression(Identifier identifier)
        {
            return Matchers.GetValueOrNone(identifier.Type)
                .Map(matcher => matcher.Of(identifier.Value))
                .ValueOr(new EmptyMatcher().Of(identifier.Value));
        }

        private static Expression<Func<Patient, bool>> GetExpression(
            IEnumerable<Identifier> identifiers,
            Expression<Func<Patient, bool>> @default)
        {
            return identifiers
                .Select(ToExpression)
                .DefaultIfEmpty(@default)
                .Aggregate((accumulate, next) => accumulate.Or(next));
        }

        public static Expression<Func<Patient, bool>> GetVerifiedExpression(IEnumerable<Identifier> identifiers)
        {
            return GetExpression(identifiers, ExpressionBuilder.False<Patient>());
        }

        public static Expression<Func<Patient, bool>> GetUnVerifiedExpression(IEnumerable<Identifier> identifiers)
        {
            return GetExpression(identifiers, ExpressionBuilder.True<Patient>());
        }
    }
}