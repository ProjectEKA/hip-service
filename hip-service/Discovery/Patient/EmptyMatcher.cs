namespace hip_service.Discovery.Patient
{
    using System;
    using System.Linq.Expressions;
    using hip_service.Discovery.Patient.models;

    public class EmptyMatcher : IIdentifierMatcher
    {
        public Expression<Func<PatientInfo, bool>> of(String value) =>
            (p) => false;
    }
}