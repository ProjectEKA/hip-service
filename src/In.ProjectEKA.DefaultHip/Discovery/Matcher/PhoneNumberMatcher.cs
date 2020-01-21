namespace In.ProjectEKA.DefaultHip.Discovery.Matcher
{
    using System;
    using System.Linq.Expressions;
    using Model;

    public class PhoneNumberMatcher : IIdentifierMatcher
    {
        public Expression<Func<Patient, bool>> Of(string value)
        {
            return patientInfo => patientInfo.PhoneNumber == value;
        }
    }
}