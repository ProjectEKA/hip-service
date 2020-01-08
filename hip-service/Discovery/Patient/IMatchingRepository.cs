using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace hip_service.Discovery.Patient
{
    public interface IMatchingRepository
    {
        Task<IQueryable<Model.Patient>> Where(Expression<Func<Model.Patient, bool>> predicate);
    }

}