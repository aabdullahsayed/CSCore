# 8. Repository and Unit of Work Patterns

## The Repository pattern

Abstracts data-access logic behind an interface, decoupling business logic from EF Core specifics and making the code easier to test and swap out later.

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() => await DbSet.AsNoTracking().ToListAsync();
    public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);
    public void Update(T entity) => DbSet.Update(entity);
    public void Remove(T entity) => DbSet.Remove(entity);
}
```

## Specialized repositories

```csharp
public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> SearchByTitleAsync(string term);
}

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Book>> SearchByTitleAsync(string term) =>
        await DbSet.Where(b => b.Title.Contains(term)).ToListAsync();
}
```

## The Unit of Work pattern

Coordinates multiple repositories under a single transaction — all their changes are committed together with one `SaveChangesAsync()` call.

```csharp
public interface IUnitOfWork : IDisposable
{
    IBookRepository Books { get; }
    IRepository<Author> Authors { get; }
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IBookRepository Books { get; }
    public IRepository<Author> Authors { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Books = new BookRepository(context);
        Authors = new Repository<Author>(context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
```

```csharp
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

Usage in a service that must update two entities atomically:

```csharp
public async Task TransferBookAsync(int bookId, int newAuthorId)
{
    var book = await _unitOfWork.Books.GetByIdAsync(bookId) ?? throw new NotFoundException("Book not found");
    book.AuthorId = newAuthorId;
    _unitOfWork.Books.Update(book);

    await _unitOfWork.SaveChangesAsync();  // one transaction for all changes made above
}
```

## Is this pattern always necessary?

There's ongoing debate in the .NET community: `DbContext` **already implements** both the Repository and Unit of Work patterns internally (`DbSet<T>` = repository, `SaveChanges()` = unit of work). For many projects, injecting `DbContext` directly into services is simpler and equally testable (using EF Core's InMemory or SQLite providers in tests).

**Use explicit Repository/UnitOfWork when:**
- You need to swap data stores (e.g., supporting both SQL Server and a document DB)
- You want a clean seam to fully mock data access in unit tests without any EF Core dependency
- Your team has a strong convention around it

**Skip it when:**
- It's a small-to-medium project and `DbContext` injection with integration tests is sufficient — avoids an extra abstraction layer with limited benefit (a critique often called "repository over DbContext is redundant").

This is a judgment call, not a hard rule — both are valid, production-proven approaches.

## Practice task

Refactor your Books API to use `IUnitOfWork` with `Books` and `Authors` repositories, and implement a method that creates an `Author` and their first `Book` in a single transaction.

**Next → `../04-expert/01-clean-architecture.md`**
