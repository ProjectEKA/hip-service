using System;
using System.Linq.Expressions;
using hip_service.Discovery.Patient.models;

namespace hip_service.Discovery.Patient
{
    public interface IIdentifierMatcher
    {
        Expression<Func<Model.Patient, bool>> Of(string value);
    }
}