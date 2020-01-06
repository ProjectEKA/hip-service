using System;
using System.Linq.Expressions;
using hip_service.Discovery.Patient.models;

namespace hip_service.Discovery.Patient
{
    public class EmptyMatcher : IIdentifierMatcher
    {
        public Expression<Func<models.Patient, bool>> Of(string value) =>
            p => false;
    }
}