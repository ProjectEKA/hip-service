namespace In.ProjectEKA.HipService.Discovery.Matcher
{
    using System;
    using System.Linq.Expressions;
    using Model;

    public interface IIdentifierMatcher
    {
        Expression<Func<Patient, bool>> Of(string value);
    }
}