using System;
using System.Linq.Expressions;

namespace hip_service.Discovery.Patient
{
    public interface IIdentifierMatcher
    {
        Expression<Func<Model.Patient, bool>> Of(string value);
    }
}