namespace hip_service.Discovery.Patient.Matcher
{
    using System;
    using System.Linq.Expressions;

    public class PhoneNumberMatcher : IIdentifierMatcher
    {
        public Expression<Func<Model.Patient, bool>> Of(string value) =>
            patientInfo => patientInfo.PhoneNumber == value;
    }
}