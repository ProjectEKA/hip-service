namespace In.ProjectEKA.DefaultHip.Discovery
{
    using System;
    using System.Linq.Expressions;
    using HipLibrary.Patient.Model;

    public class EmptyMatcher : IIdentifierMatcher
    {
        public Expression<Func<Patient, bool>> Of(string value)
        {
            return p => false;
        }
    }
}