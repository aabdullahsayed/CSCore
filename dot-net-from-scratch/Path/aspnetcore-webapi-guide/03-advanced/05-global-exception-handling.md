# 5. Global Exception Handling

## Why centralize error handling?

Without it, unhandled exceptions leak stack traces to clients (a security risk) and every controller ends up littered with try/catch blocks. A single exception-handling middleware gives consistent, safe error responses everywhere.

## Approach 1: `IExceptionHandler` (recommended, .NET 8+)

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred");

        var (statusCode, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        }, cancellationToken);

        return true;
    }
}
```

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
...
app.UseExceptionHandler();
```

## Approach 2: custom middleware (works on all versions)

```csharp
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred",
                Detail = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                    ? ex.ToString() : null
            };
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
```

Register it **first** in the pipeline so it wraps everything else.

## Custom domain exceptions

```csharp
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
```

```csharp
public async Task<Book> GetByIdOrThrowAsync(int id) =>
    await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"Book {id} was not found.");
```

## `ProblemDetails` — the standard error format (RFC 9457)

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Resource not found",
  "status": 404,
  "detail": "Book 42 was not found.",
  "instance": "/api/books/42"
}
```

Using this consistent shape makes error handling predictable for API consumers.

## Never leak details in production

Only return stack traces / exception messages when `app.Environment.IsDevelopment()` is true. In production, log the full exception server-side but return a generic message to the client.

## Practice task

Create `NotFoundException` and `ValidationException`, throw them from your service layer instead of returning `null`/`BadRequest` directly, and wire up a global handler that converts them to appropriate `ProblemDetails` responses.

**Next → `06-caching.md`**
