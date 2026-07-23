# 1. Dependency Injection (DI)

## Why DI?

Hardcoding dependencies (like `new BookRepository()` inside a controller) makes code hard to test and tightly coupled. ASP.NET Core has a **built-in DI container** — you register services once, and the framework injects them wherever they're needed.

## The three lifetimes

```csharp
builder.Services.AddTransient<IEmailSender, EmailSender>();  // new instance every time it's requested
builder.Services.AddScoped<IBookRepository, BookRepository>(); // one instance per HTTP request
builder.Services.AddSingleton<ICacheService, CacheService>();  // one instance for the app's lifetime
```

| Lifetime | New instance... | Typical use |
|---|---|---|
| **Transient** | every injection | lightweight, stateless services |
| **Scoped** | once per request | `DbContext`, repositories, per-request state |
| **Singleton** | once for the app | caching, configuration, background state |

⚠️ **Common bug**: injecting a Scoped service into a Singleton causes a runtime error (captive dependency). Never do this.

## Defining and registering a service

```csharp
public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task AddAsync(Book book);
}

public class BookRepository : IBookRepository
{
    private readonly List<Book> _books = new();

    public Task<IEnumerable<Book>> GetAllAsync() => Task.FromResult(_books.AsEnumerable());
    public Task<Book?> GetByIdAsync(int id) => Task.FromResult(_books.FirstOrDefault(b => b.Id == id));
    public Task AddAsync(Book book) { _books.Add(book); return Task.CompletedTask; }
}
```

```csharp
// Program.cs
builder.Services.AddScoped<IBookRepository, BookRepository>();
```

## Constructor injection in a controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _repository;

    public BooksController(IBookRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAll()
        => Ok(await _repository.GetAllAsync());
}
```

The framework resolves `IBookRepository` automatically at request time — you never call `new BookRepository()` yourself.

## Why this matters for testing

Because the controller depends on an **interface**, unit tests can inject a fake/mock implementation instead of a real database — no network or database needed. This is covered in depth in `03-advanced/07-unit-and-integration-testing.md`.

## Other DI features worth knowing

- **`IServiceCollection` extension methods** — group related registrations into a method like `AddApplicationServices()` to keep `Program.cs` clean.
- **Factory registration** — `AddScoped<IFoo>(sp => new Foo(sp.GetRequiredService<IBar>()))` for services needing custom construction logic.
- **`IOptions<T>`** — inject strongly-typed configuration (see `05-configuration-and-environments.md`).

## Practice task

Extract the in-memory `Books` list from the Beginner CRUD controller into an `IBookRepository`/`BookRepository` pair, register it as Scoped, and inject it into `BooksController`.

**Next → `02-entity-framework-core.md`**
