namespace hip_service.Discovery.Patient
{
    using System;
    using System.Linq.Expressions;
    using hip_service.Discovery.Patient.models;

    public interface IIdentifierMatcher
    {
        Expression<Func<PatientInfo, bool>> of(String value);
    }
}