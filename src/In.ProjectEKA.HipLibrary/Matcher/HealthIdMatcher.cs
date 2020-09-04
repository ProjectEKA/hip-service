using System;
using System.Linq.Expressions;

namespace In.ProjectEKA.HipLibrary.Matcher
{
    public class HealthIdMatcher : IIdentifierMatcher
    {
        public Expression<Func<Patient.Model.Patient, bool>> Of(string value)
        {
            return patientInfo => patientInfo.HealthId == value;
        }
    }
}