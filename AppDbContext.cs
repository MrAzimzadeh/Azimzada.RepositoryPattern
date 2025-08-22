using Microsoft.EntityFrameworkCore;
using System;

namespace Azimzada.RepositoryPattern.Example
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ExampleEntity> ExampleEntities { get; set; }
    }

    public class ExampleEntity : Azimzada.RepositoryPattern.ISoftDelete, Azimzada.RepositoryPattern.ILocalizedEntity, Azimzada.RepositoryPattern.IAuditableEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string LangCode { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Name { get; set; }
    }
}
