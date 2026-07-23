# 2. CQRS and MediatR

## What is CQRS?

**Command Query Responsibility Segregation** — separate the models/paths used for **writes** (Commands: create/update/delete) from those used for **reads** (Queries: get data). They don't have to share the same DTOs, validation, or even data source.

Benefits:
- Read paths can be optimized independently (e.g., simpler flattened DTOs, no tracking, even a separate read replica database)
- Write paths can enforce strict business rules without being shaped by UI read requirements
- Keeps handlers small and single-purpose instead of bloated services with a dozen methods

## MediatR — implementing CQRS cleanly

MediatR is a popular library implementing the in-process mediator pattern: controllers send a request object, and a matching handler processes it — controllers never call services directly.

```bash
dotnet add package MediatR
```

```csharp
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

## Defining a Command

```csharp
public record CreateBookCommand(string Title, string Author, decimal Price) : IRequest<int>;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, int>
{
    private readonly IBookRepository _repository;
    public CreateBookCommandHandler(IBookRepository repository) => _repository = repository;

    public async Task<int> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = new Book(request.Title, request.Price);
        await _repository.AddAsync(book);
        return book.Id;
    }
}
```

## Defining a Query

```csharp
public record GetBookByIdQuery(int Id) : IRequest<BookResponseDto?>;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookResponseDto?>
{
    private readonly AppDbContext _context;
    public GetBookByIdQueryHandler(AppDbContext context) => _context = context;

    public async Task<BookResponseDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Books
            .Where(b => b.Id == request.Id)
            .Select(b => new BookResponseDto { Id = b.Id, Title = b.Title, Price = b.Price })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
```

Note the query handler projects directly to a DTO with `.Select()` — it bypasses loading full entities, which is more efficient for read-only paths.

## Slim controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;
    public BooksController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _mediator.Send(new GetBookByIdQuery(id));
        return book is null ? NotFound() : Ok(book);
    }
}
```

Controllers become thin — they just translate HTTP into a message and send it. All logic lives in focused, independently testable handlers.

## Cross-cutting concerns with Pipeline Behaviors

MediatR lets you wrap every request with reusable middleware-like behaviors — logging, validation, caching, transactions:

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        var response = await next();
        _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
        return response;
    }
}
```

```csharp
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

## Validation behavior with FluentValidation

```bash
dotnet add package FluentValidation.DependencyInjectionExtensions
```

```csharp
public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

A `ValidationBehavior<TRequest, TResponse>` pipeline behavior can automatically run all matching validators before every command reaches its handler — centralizing validation instead of repeating it per-handler.

## Is CQRS/MediatR always worth it?

Like Clean Architecture, this adds indirection. It shines in larger systems with complex business logic and many use cases. For small CRUD APIs, plain services are often simpler and just as maintainable — introduce MediatR when the number of use cases and cross-cutting concerns (logging, validation, caching, transactions) starts making services unwieldy.

## Practice task

Convert `CreateBook`, `UpdateBook`, `DeleteBook`, `GetBookById`, and `GetAllBooks` into MediatR Commands/Queries with handlers, and add a `LoggingBehavior` and a FluentValidation `ValidationBehavior` to the pipeline.

**Next → `03-microservices-basics.md`**
