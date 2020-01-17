namespace In.ProjectEKA.HipService.Discovery.Patient.Matcher
{
    using System;
    using System.Linq.Expressions;
    using Model;

    public class EmptyMatcher : IIdentifierMatcher
    {
        public Expression<Func<Patient, bool>> Of(string value)
        {
            return p => false;
        }
    }
}