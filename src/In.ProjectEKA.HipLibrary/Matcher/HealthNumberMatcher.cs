namespace In.ProjectEKA.HipLibrary.Matcher
{
    using System;
    using System.Linq.Expressions;
    using Patient.Model;

    public class HealthNumberMatcher : IIdentifierMatcher
    {
        public Expression<Func<Patient, bool>> Of(string value)
        {
            return patientInfo => patientInfo.HealthNumber == value;
        }
    }
}