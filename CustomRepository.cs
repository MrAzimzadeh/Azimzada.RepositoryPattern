using Azimzada.RepositoryPattern;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Azimzada.RepositoryPattern.Example
{
    public class CustomRepository<T> : BaseRepository<T> where T : class
    {
        public CustomRepository(DbContext context) : base(context) { }

        protected override async Task BeforeSaveChangesAsync()
        {
            // Custom logic before saving
            await Task.CompletedTask;
        }

        protected override async Task AfterSaveChangesAsync()
        {
            // Custom logic after saving
            await Task.CompletedTask;
        }

        public override async Task<T> AddAsync(T entity)
        {
            // Custom add logic
            return await base.AddAsync(entity);
        }
    }
}
