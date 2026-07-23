# 6. Filters

## Middleware vs Filters — what's the difference?

- **Middleware** wraps the *entire* HTTP pipeline — applies to all requests regardless of framework.
- **Filters** run *inside* the MVC/API pipeline, closer to the controller/action, and are aware of MVC-specific context (action arguments, model binding results, the result being returned).

Use filters when you need MVC-aware behavior: validating model state, wrapping specific actions/controllers, modifying action results, etc.

## The filter types (in execution order)

```
Request
  → Authorization Filters   (is the user allowed?)
  → Resource Filters        (before/after model binding)
  → Action Filters          (before/after the action method runs)
  → Exception Filters       (catch exceptions from action/model binding)
  → Result Filters          (before/after the result is executed)
Response
```

## Action Filter example — logging

```csharp
public class LogActionFilter : IActionFilter
{
    private readonly ILogger<LogActionFilter> _logger;
    public LogActionFilter(ILogger<LogActionFilter> logger) => _logger = logger;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("Executing {Action} with args: {@Args}",
            context.ActionDescriptor.DisplayName, context.ActionArguments);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("Executed {Action}, result: {Result}",
            context.ActionDescriptor.DisplayName, context.Result?.GetType().Name);
    }
}
```

Apply it:
```csharp
// Just this action
[ServiceFilter(typeof(LogActionFilter))]
[HttpGet]
public IActionResult Get() => Ok();

// Register the filter as a service first
builder.Services.AddScoped<LogActionFilter>();
```

Or apply globally to every action:
```csharp
builder.Services.AddControllers(options =>
{
    options.Filters.Add<LogActionFilter>();
});
```

## Exception Filter example

```csharp
public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;
    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) => _logger = logger;

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception");

        context.Result = new ObjectResult(new { message = "An unexpected error occurred." })
        {
            StatusCode = 500
        };
        context.ExceptionHandled = true;
    }
}
```

> In modern ASP.NET Core, the preferred way to handle errors globally is `IExceptionHandler` (covered in the Advanced section) rather than an exception filter — but exception filters are still useful for action/controller-specific error handling.

## Simple attribute-based filters (no DI needed)

```csharp
public class SimpleTimingFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Items["StartTime"] = DateTime.UtcNow;
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var start = (DateTime)context.HttpContext.Items["StartTime"]!;
        var elapsed = DateTime.UtcNow - start;
        context.HttpContext.Response.Headers["X-Response-Time-Ms"] = elapsed.TotalMilliseconds.ToString("F0");
    }
}
```

Apply directly as an attribute:
```csharp
[SimpleTimingFilter]
[HttpGet]
public IActionResult Get() => Ok();
```

## Where filters fit in a real project

- **Authorization filters** → handled mostly by `[Authorize]` (see Advanced section on auth).
- **Action filters** → cross-cutting logic like logging, request/response transformation.
- **Exception filters** → fallback error handling per controller (global handling is usually better, see Advanced).
- **Result filters** → modifying the outgoing response shape (e.g., wrapping all responses in an envelope `{ "data": ..., "meta": ... }`).

---
**Next:** `07-Async-Await-Best-Practices.md`
