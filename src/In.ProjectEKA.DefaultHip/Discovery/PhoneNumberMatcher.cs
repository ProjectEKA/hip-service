namespace In.ProjectEKA.DefaultHip.Discovery
{
    using System;
    using System.Linq.Expressions;
    using HipLibrary.Patient.Model;

    public class PhoneNumberMatcher : IIdentifierMatcher
    {
        public Expression<Func<Patient, bool>> Of(string value)
        {
            return patientInfo => patientInfo.PhoneNumber == value;
        }
    }
}