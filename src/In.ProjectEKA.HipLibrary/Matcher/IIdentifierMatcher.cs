namespace In.ProjectEKA.HipLibrary.Matcher
{
    using System;
    using System.Linq.Expressions;
    using Patient.Model;

    public interface IIdentifierMatcher
    {
        Expression<Func<Patient, bool>> Of(string value);
    }
}