# 1. Clean Architecture

## The core idea

Separate your code into layers so that **business logic doesn't depend on infrastructure details** (databases, frameworks, external APIs). Dependencies point inward, toward the domain.

```
┌─────────────────────────────────────────────┐
│  Infrastructure (EF Core, external APIs)     │
│  ┌─────────────────────────────────────┐    │
│  │  Application (use cases, services)    │    │
│  │  ┌─────────────────────────────┐     │    │
│  │  │  Domain (entities, rules)    │     │    │
│  │  └─────────────────────────────┘     │    │
│  └─────────────────────────────────────┘    │
│  API (controllers, Program.cs)               │
└─────────────────────────────────────────────┘
```

**The Dependency Rule**: inner layers know nothing about outer layers. `Domain` has zero references to EF Core or ASP.NET Core.

## Project structure

```
MyApp.sln
├── src/
│   ├── MyApp.Domain/           # entities, value objects, domain events, interfaces
│   ├── MyApp.Application/      # use cases, DTOs, service interfaces, validators
│   ├── MyApp.Infrastructure/   # EF Core, repositories, email/file/external services
│   └── MyApp.Api/              # controllers, middleware, Program.cs
└── tests/
    ├── MyApp.Domain.Tests/
    ├── MyApp.Application.Tests/
    └── MyApp.Api.IntegrationTests/
```

Project references flow one direction:

```
Api → Application → Domain
Api → Infrastructure → Application, Domain
```

`Infrastructure` implements interfaces defined in `Domain`/`Application` — this is the **Dependency Inversion Principle** in action.

## Domain layer example

```csharp
// MyApp.Domain/Entities/Book.cs
public class Book
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public decimal Price { get; private set; }

    private Book() { Title = string.Empty; }  // for EF Core

    public Book(string title, decimal price)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
        if (price <= 0) throw new ArgumentException("Price must be positive.");
        Title = title;
        Price = price;
    }

    public void ApplyDiscount(decimal percent)
    {
        if (percent is < 0 or > 100) throw new ArgumentException("Invalid discount percent.");
        Price -= Price * (percent / 100);
    }
}
```

Notice: **no `[Key]` attributes, no EF Core references, no `[Required]` data annotations** — the domain entity enforces its own invariants through code, independent of any framework. This is called a "rich domain model," contrasted with an "anemic" one that's just a plain property bag.

```csharp
// MyApp.Domain/Interfaces/IBookRepository.cs
public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id);
    Task AddAsync(Book book);
}
```

## Application layer example

```csharp
// MyApp.Application/Books/CreateBookHandler.cs
public class CreateBookHandler
{
    private readonly IBookRepository _repository;
    public CreateBookHandler(IBookRepository repository) => _repository = repository;

    public async Task<int> HandleAsync(CreateBookRequest request)
    {
        var book = new Book(request.Title, request.Price);
        await _repository.AddAsync(book);
        return book.Id;
    }
}
```

## Infrastructure layer example

```csharp
// MyApp.Infrastructure/Repositories/BookRepository.cs
public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;
    public BookRepository(AppDbContext context) => _context = context;

    public async Task<Book?> GetByIdAsync(int id) => await _context.Books.FindAsync(id);
    public async Task AddAsync(Book book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
    }
}
```

## API layer example

```csharp
// MyApp.Api/Controllers/BooksController.cs
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly CreateBookHandler _handler;
    public BooksController(CreateBookHandler handler) => _handler = handler;

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookRequest request)
    {
        var id = await _handler.HandleAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }
}
```

## Why this pays off

- **Testability** — `Domain` and `Application` layers can be unit-tested with zero database/framework dependencies.
- **Swappable infrastructure** — switch from SQL Server to PostgreSQL, or EF Core to Dapper, by only touching `Infrastructure`.
- **Clear boundaries** — new team members immediately know where business rules live vs. plumbing code.

## When it's overkill

For small APIs, CRUD-heavy admin tools, or prototypes, this much layering adds ceremony without payoff. Reach for Clean Architecture when the domain has real business rules worth protecting and the project is expected to grow and be maintained for years.

## Practice task

Restructure your Books API into `Domain`, `Application`, `Infrastructure`, and `Api` projects, moving validation logic into the `Book` entity's constructor instead of data annotations.

**Next → `02-cqrs-and-mediatr.md`**
