# 3. Global Error Handling and Logging

## Why centralize error handling?

Without it, every controller action needs its own try/catch, leading to inconsistent error responses and duplicated code. A single global handler guarantees every unhandled exception returns a consistent, safe response.

## The modern approach: IExceptionHandler (.NET 8+)

```csharp
using Microsoft.AspNetCore.Diagnostics;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred");

        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new
        {
            title,
            status = statusCode,
            traceId = httpContext.TraceIdentifier
        }, cancellationToken);

        return true;   // exception handled, don't rethrow
    }
}
```

Register:
```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();   // enables the standard ProblemDetails format

var app = builder.Build();
app.UseExceptionHandler();   // must be one of the first middleware registered
```

## Custom exception types for domain errors

```csharp
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }
    public ValidationException(IDictionary<string, string[]> errors) : base("Validation failed")
    {
        Errors = errors;
    }
}
```

Throw them from your service layer:
```csharp
public async Task<BookResponseDto> GetByIdAsync(int id)
{
    var book = await _repo.GetByIdAsync(id)
        ?? throw new NotFoundException($"Book {id} not found.");

    return book.ToDto();
}
```

Handle them centrally in `GlobalExceptionHandler`, mapping each custom exception to the right status code — keeps controllers clean of try/catch entirely.

## ProblemDetails — the standard error format (RFC 7807)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Resource not found",
  "status": 404,
  "traceId": "00-8f3a2b1c..."
}
```

ASP.NET Core's `[ApiController]` already returns this shape for validation errors; using it for all your error responses keeps the API consistent and predictable for client developers.

## Structured logging with Serilog (recommended over default logging for production)

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
```

```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
```

Log with structured properties (never string-concatenate values into the message):
```csharp
_logger.LogInformation("Book {BookId} created by user {UserId}", book.Id, userId);
// ✅ good — BookId and UserId become searchable structured fields

_logger.LogInformation($"Book {book.Id} created by user {userId}");
// ❌ avoid — loses structure, harder to query in log aggregators
```

## Log levels — when to use which

| Level | Use for |
|---|---|
| `Trace` | Extremely detailed diagnostic info, disabled in production |
| `Debug` | Development-time diagnostic detail |
| `Information` | Normal application flow (request started, resource created) |
| `Warning` | Unexpected but recoverable situations |
| `Error` | Failures that affect the current operation |
| `Critical` | Failures requiring immediate attention (app crash, data loss risk) |

## Correlation / trace IDs

`HttpContext.TraceIdentifier` gives you a unique ID per request — include it in every log line and error response so you can trace a single request's full journey through logs (especially valuable once you have multiple services, see the Microservices file).

---
**Next:** `04-Caching.md`
