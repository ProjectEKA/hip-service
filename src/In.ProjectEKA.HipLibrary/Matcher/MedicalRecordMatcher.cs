namespace In.ProjectEKA.HipLibrary.Matcher
{
    using System;
    using System.Linq.Expressions;
    using Patient.Model;

    public class MedicalRecordMatcher : IIdentifierMatcher
    {
        public Expression<Func<Patient, bool>> Of(string value)
        {
            return patient => patient.Identifier == value;
        }
    }
}