namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Model;

    public interface IMatchingRepository
    {
        Task<IQueryable<Patient>> Where(Expression<Func<Patient, bool>> predicate);
    }
}