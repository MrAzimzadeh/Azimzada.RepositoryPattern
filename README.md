# Azimzada.RepositoryPattern

A fully generic, extensible EF Core repository pattern for .NET 8.

## Features
- Generic CRUD operations
- Overridable methods and hooks
- Pagination, sorting, and Include/ThenInclude support
- Soft delete (`ISoftDelete`)
- Language-aware filtering (`ILocalizedEntity`)
- Audit fields (`IAuditableEntity`)
- Batch operations
- Async methods
- Works with any EF Core DbContext entity

## Usage

### 1. Define your entity
```csharp
public class ExampleEntity : ISoftDelete, ILocalizedEntity, IAuditableEntity
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
```

### 2. Setup DbContext
```csharp
public class AppDbContext : DbContext
{
    public DbSet<ExampleEntity> ExampleEntities { get; set; }
}
```

### 3. Use the repository
```csharp
var dbContext = new AppDbContext(options);
var repo = new BaseRepository<ExampleEntity>(dbContext);

// Add
await repo.AddAsync(new ExampleEntity { Name = "Test", LangCode = "en" });

// Get all
var all = await repo.GetAllAsync(langCode: "en", page: 1, pageSize: 10);

// Soft delete
await repo.SoftDeleteAsync(1);

// Batch update
await repo.UpdateRangeAsync(listOfEntities);
```

### 4. Override methods
```csharp
public class CustomRepository<T> : BaseRepository<T> where T : class
{
    public CustomRepository(DbContext context) : base(context) { }

    protected override async Task BeforeSaveChangesAsync()
    {
        // Custom logic
        await Task.CompletedTask;
    }
}
```

## License
MIT

## Author
[MrAzimzada](https://github.com/MrAzimzada)
# Azimzada.RepositoryPattern
