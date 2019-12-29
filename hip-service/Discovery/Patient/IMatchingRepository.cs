using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using hip_service.Discovery.Patient.models;

namespace hip_service.Discovery.Patient
{
    public interface IMatchingRepository
    {
        Task<IQueryable<PatientInfo>> Where(Expression<Func<PatientInfo, bool>> predicate);
    }

}