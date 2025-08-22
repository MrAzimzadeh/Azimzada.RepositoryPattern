using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Azimzada.RepositoryPattern
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Hooks
        protected virtual Task BeforeSaveChangesAsync() => Task.CompletedTask;
        protected virtual Task AfterSaveChangesAsync() => Task.CompletedTask;

        public virtual async Task<T> GetByIdAsync(object id, string langCode = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
                query = query.Include(include);

            var entity = await query.FindAsync(id);

            if (entity is ISoftDelete softDelete && softDelete.IsDeleted)
                return null;

            if (langCode != null && entity is ILocalizedEntity localized && localized.LangCode != langCode)
                return null;

            return entity;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(string langCode = null, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? page = null, int? pageSize = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
                query = query.Where(e => !(e as ISoftDelete).IsDeleted);

            if (langCode != null && typeof(ILocalizedEntity).IsAssignableFrom(typeof(T)))
                query = query.Where(e => (e as ILocalizedEntity).LangCode == langCode);

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page.HasValue && pageSize.HasValue)
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            return await query.ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity is IAuditableEntity auditable)
            {
                auditable.CreatedAt = DateTime.UtcNow;
                // auditable.CreatedBy = ... // set from context if available
            }
            await _dbSet.AddAsync(entity);
            await SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            if (entity is IAuditableEntity auditable)
            {
                auditable.UpdatedAt = DateTime.UtcNow;
                // auditable.UpdatedBy = ... // set from context if available
            }
            _dbSet.Update(entity);
            await SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return;
            _dbSet.Remove(entity);
            await SaveChangesAsync();
        }

        public virtual async Task SoftDeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity is ISoftDelete softDelete)
            {
                softDelete.IsDeleted = true;
                await UpdateAsync(entity);
            }
        }

        public virtual async Task ActivateAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity is ISoftDelete softDelete)
            {
                softDelete.IsDeleted = false;
                await UpdateAsync(entity);
            }
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is IAuditableEntity auditable)
                    auditable.UpdatedAt = DateTime.UtcNow;
            }
            _dbSet.UpdateRange(entities);
            await SaveChangesAsync();
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<object> ids)
        {
            var entities = new List<T>();
            foreach (var id in ids)
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                    entities.Add(entity);
            }
            _dbSet.RemoveRange(entities);
            await SaveChangesAsync();
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            await BeforeSaveChangesAsync();
            var result = await _context.SaveChangesAsync();
            await AfterSaveChangesAsync();
            return result;
        }
    }
}
