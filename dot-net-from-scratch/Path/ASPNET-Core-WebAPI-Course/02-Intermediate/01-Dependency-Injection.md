# 1. Dependency Injection (DI)

## The problem DI solves

Without DI, classes create their own dependencies:

```csharp
public class BooksController : ControllerBase
{
    private readonly BookRepository _repo = new BookRepository(); // tightly coupled!
}
```

This makes testing hard (can't swap in a fake repository) and creates rigid coupling between classes.

## The DI way

```csharp
public class BooksController : ControllerBase
{
    private readonly IBookRepository _repo;

    public BooksController(IBookRepository repo)   // dependency is "injected"
    {
        _repo = repo;
    }
}
```

ASP.NET Core has a built-in **DI container**. You register an interface → implementation mapping once, and the framework automatically supplies (injects) the right implementation wherever it's needed — constructors, mostly.

## Registering services in Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddControllers();

var app = builder.Build();
```

Now any controller that asks for `IBookRepository` in its constructor automatically receives a `BookRepository` instance — no `new` keyword needed.

## The three lifetimes

| Lifetime | Instance created | Use for |
|---|---|---|
| `AddTransient` | New instance every time it's requested | Lightweight, stateless services |
| `AddScoped` | One instance per HTTP request | Most services — repositories, DbContext, business logic |
| `AddSingleton` | One instance for the entire app lifetime | Caches, configuration objects, stateless utility services |

```csharp
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
```

> ⚠️ Common bug: injecting a **Scoped** service (like `DbContext`) into a **Singleton** throws a runtime error, because the singleton would outlive many requests while holding onto a single DbContext. Match lifetimes carefully — when in doubt, use `Scoped`.

## Interfaces as contracts

```csharp
public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<Book> CreateAsync(Book book);
    Task<bool> UpdateAsync(Book book);
    Task<bool> DeleteAsync(int id);
}

public class BookRepository : IBookRepository
{
    private static readonly List<Book> _books = new();

    public Task<IEnumerable<Book>> GetAllAsync() =>
        Task.FromResult(_books.AsEnumerable());

    public Task<Book?> GetByIdAsync(int id) =>
        Task.FromResult(_books.FirstOrDefault(b => b.Id == id));

    // ... other methods
}
```

Why bother with an interface for a simple repository? Because:
1. You can swap `BookRepository` for `SqlBookRepository` later without touching controllers.
2. You can mock `IBookRepository` in unit tests without hitting a real database.

## Constructor injection with multiple dependencies

```csharp
public class BooksController : ControllerBase
{
    private readonly IBookRepository _repo;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookRepository repo, ILogger<BooksController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAll()
    {
        _logger.LogInformation("Fetching all books");
        return Ok(await _repo.GetAllAsync());
    }
}
```

Note: `ILogger<T>` is a built-in service — no manual registration needed, it's auto-registered by the framework.

## Options pattern (injecting configuration)

```csharp
// appsettings.json
// "BookStoreSettings": { "MaxPageSize": 50 }

public class BookStoreSettings
{
    public int MaxPageSize { get; set; }
}

// Program.cs
builder.Services.Configure<BookStoreSettings>(
    builder.Configuration.GetSection("BookStoreSettings"));

// Usage in a class
public class BooksController : ControllerBase
{
    private readonly BookStoreSettings _settings;
    public BooksController(IOptions<BookStoreSettings> options)
    {
        _settings = options.Value;
    }
}
```

---
**Next:** `02-EFCore-Getting-Started.md` — replacing the in-memory list with a real database.
