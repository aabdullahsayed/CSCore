# 6. Logging

## Built-in logging

ASP.NET Core ships with `ILogger<T>`, injectable anywhere via DI:

```csharp
public class BooksController : ControllerBase
{
    private readonly ILogger<BooksController> _logger;
    public BooksController(ILogger<BooksController> logger) => _logger = logger;

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Book>> GetById(int id)
    {
        _logger.LogInformation("Fetching book with id {BookId}", id);

        var book = await _repository.GetByIdAsync(id);
        if (book is null)
        {
            _logger.LogWarning("Book with id {BookId} was not found", id);
            return NotFound();
        }
        return Ok(book);
    }
}
```

⚠️ Always use **structured logging** with placeholders (`{BookId}`) instead of string interpolation (`$"..."`) — this lets log providers index and query fields, and avoids allocating strings when the log level is disabled.

## Log levels

| Level | Use for |
|---|---|
| `Trace` | Extremely detailed, dev-only diagnostics |
| `Debug` | Debugging info, not for production |
| `Information` | Normal application flow |
| `Warning` | Unexpected but recoverable situations |
| `Error` | Failures affecting the current operation |
| `Critical` | App-wide failures needing immediate attention |

## Configuring log levels

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "MyFirstApi.Controllers": "Debug"
    }
  }
}
```

## Structured logging with Serilog (industry standard)

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

Serilog can also write to **Seq**, **Elasticsearch**, **Application Insights**, or **Datadog** — useful when you move to centralized logging in production (see `04-expert/05-performance-optimization.md`).

## Correlation IDs

For tracing a single request across multiple log lines (and across microservices), attach a correlation/trace ID:

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Correlation-Id"] = context.TraceIdentifier;
    await next(context);
});
```

ASP.NET Core's `HttpContext.TraceIdentifier` is unique per request by default and appears automatically in structured logs when using `Enrich.FromLogContext()`.

## Practice task

Add Serilog to your Books API, configure it to log to both console and a rolling file, and add structured logging (`_logger.LogInformation`) to each CRUD action.

**Next → `07-async-await.md`**
