# 4. The Middleware Pipeline

## What is middleware?

Middleware are components chained together to process every HTTP request/response. Each one can:

1. Do something before passing control to the next component
2. Call the next middleware in the chain (`await next(context)`)
3. Do something after that component returns

```
Request  →  [Exception Handling] → [HTTPS Redirect] → [Routing] → [Auth] → [Controller]
Response ←  [Exception Handling] ← [HTTPS Redirect] ← [Routing] ← [Auth] ← [Controller]
```

**Order matters.** Middleware registered earlier in `Program.cs` runs first on the way in and last on the way out.

## Typical pipeline order

```csharp
var app = builder.Build();

app.UseExceptionHandler();       // catch unhandled exceptions first
app.UseHttpsRedirection();       // force HTTPS
app.UseStaticFiles();            // serve wwwroot files, if any
app.UseRouting();                // determine which endpoint matches
app.UseCors("MyPolicy");         // must be after UseRouting, before UseAuthorization
app.UseAuthentication();         // who are you?
app.UseAuthorization();          // are you allowed?
app.MapControllers();            // execute the matched endpoint

app.Run();
```

## Writing custom middleware

**Inline (for quick, simple logic):**

```csharp
app.Use(async (context, next) =>
{
    var start = DateTime.UtcNow;
    await next(context);
    var elapsed = DateTime.UtcNow - start;
    Console.WriteLine($"{context.Request.Method} {context.Request.Path} took {elapsed.TotalMilliseconds}ms");
});
```

**As a reusable class (recommended for anything nontrivial):**

```csharp
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await _next(context);
        sw.Stop();
        _logger.LogInformation("{Method} {Path} responded {StatusCode} in {Elapsed}ms",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}

public static class RequestTimingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder) =>
        builder.UseMiddleware<RequestTimingMiddleware>();
}
```

```csharp
// Program.cs
app.UseRequestTiming();
```

## `Use` vs `Run` vs `Map`

| Method | Behavior |
|---|---|
| `app.Use(...)` | Runs, then calls the next middleware |
| `app.Run(...)` | Terminal — does not call next; ends the pipeline |
| `app.Map("/path", ...)` | Branches the pipeline based on request path |

## CORS (Cross-Origin Resource Sharing)

If your front-end runs on a different origin (e.g., `http://localhost:3000` calling `https://localhost:7123`), you must explicitly allow it:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// after UseRouting, before UseAuthorization
app.UseCors("MyPolicy");
```

Avoid `AllowAnyOrigin()` combined with credentials in production — it's a security risk.

## Practice task

Write a custom middleware that adds a response header `X-Response-Time-Ms` with the request's processing time, and register it as the very first middleware in the pipeline.

**Next → `05-configuration-and-environments.md`**
