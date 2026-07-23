# 2. Entity Framework Core (EF Core)

EF Core is Microsoft's official ORM — it maps C# classes to database tables and lets you query with LINQ instead of raw SQL.

## Install packages

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
# For local dev without SQL Server, use SQLite instead:
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

## Define your entity and DbContext

```csharp
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Price).HasColumnType("decimal(18,2)");
        });
    }
}
```

## Register the DbContext

```csharp
// appsettings.json
// "ConnectionStrings": { "Default": "Server=(localdb)\\mssqllocaldb;Database=BooksDb;Trusted_Connection=True;" }

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
```

## Migrations — evolving your schema

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

- `migrations add` generates a C# file describing the schema change.
- `database update` applies pending migrations to the actual database.
- Run `dotnet ef migrations add <Name>` again after every model change.

## Querying with LINQ

```csharp
public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;
    public BookRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Book>> GetAllAsync() =>
        await _context.Books.AsNoTracking().ToListAsync();

    public async Task<Book?> GetByIdAsync(int id) =>
        await _context.Books.FindAsync(id);

    public async Task<IEnumerable<Book>> SearchAsync(string term) =>
        await _context.Books
            .Where(b => b.Title.Contains(term))
            .OrderBy(b => b.Title)
            .ToListAsync();

    public async Task AddAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
}
```

- **`AsNoTracking()`** — use for read-only queries; skips EF's change-tracking overhead.
- **`SaveChangesAsync()`** — must be called to persist Add/Update/Remove operations; queries alone don't write to the DB.

## Relationships

```csharp
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Book> Books { get; set; } = new();
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
}
```

```csharp
// Eager-loading related data
var books = await _context.Books.Include(b => b.Author).ToListAsync();
```

## Practice task

1. Convert the in-memory `BookRepository` from the previous file to use `AppDbContext` and SQLite.
2. Add an `Author` entity with a one-to-many relationship to `Book`.
3. Create and apply a migration, then confirm data persists across app restarts.

**Next → `03-model-validation.md`**
