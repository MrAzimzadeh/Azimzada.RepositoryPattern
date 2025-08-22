using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Azimzada.RepositoryPattern
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(object id, string langCode = null, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync(string langCode = null, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? page = null, int? pageSize = null, params Expression<Func<T, object>>[] includes);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(object id);
        Task SoftDeleteAsync(object id);
        Task ActivateAsync(object id);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task DeleteRangeAsync(IEnumerable<object> ids);
        Task<int> SaveChangesAsync();
    }
}
