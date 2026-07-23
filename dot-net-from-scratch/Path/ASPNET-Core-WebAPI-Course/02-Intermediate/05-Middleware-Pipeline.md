# 5. The Middleware Pipeline

## What is middleware?

Middleware are components chained together that process every HTTP request/response, one after another, in the order they're registered:

```
Request  → [Middleware 1] → [Middleware 2] → [Middleware 3] → Controller
Response ← [Middleware 1] ← [Middleware 2] ← [Middleware 3] ← Controller
```

Each middleware can:
- Do something before passing control to the next one (`await next(context)`)
- Do something after the next one returns
- Short-circuit the pipeline entirely (e.g., return 401 without ever reaching the controller)

## Built-in middleware you already use

```csharp
app.UseHttpsRedirection();   // redirects HTTP → HTTPS
app.UseAuthentication();     // identifies who the caller is
app.UseAuthorization();      // checks if they're allowed to do this
app.MapControllers();        // routes matched requests to controller actions
```

**Order matters.** `UseAuthentication()` must come before `UseAuthorization()`, and both typically come before `MapControllers()`.

## Writing custom inline middleware

```csharp
app.Use(async (context, next) =>
{
    var start = DateTime.UtcNow;
    Console.WriteLine($"→ {context.Request.Method} {context.Request.Path}");

    await next(context);   // pass control down the pipeline

    var elapsed = DateTime.UtcNow - start;
    Console.WriteLine($"← {context.Response.StatusCode} ({elapsed.TotalMilliseconds}ms)");
});
```

## Writing a reusable middleware class

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
            context.Request.Method, context.Request.Path,
            context.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}

// Extension method for a clean Program.cs registration
public static class RequestTimingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder app) =>
        app.UseMiddleware<RequestTimingMiddleware>();
}
```

Register it:
```csharp
app.UseRequestTiming();
```

## Short-circuiting example — a simple API key check

```csharp
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.TryGetValue("X-Api-Key", out var key) || key != "secret123")
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new { message = "Invalid API key" });
        return;   // note: next() is never called — pipeline stops here
    }

    await next(context);
});
```

(In practice you'd use real authentication — covered in the Advanced section — but this illustrates the short-circuit pattern clearly.)

## CORS — a middleware you'll need almost every project

If your API is called from a browser-based front-end on a different origin (e.g., React app on `localhost:3000` calling API on `localhost:5000`), you need CORS.

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ... after app.Build()
app.UseCors("AllowFrontend");
```

Place `UseCors` before `UseAuthorization` and `MapControllers`.

## Typical middleware order in a real app

```csharp
app.UseExceptionHandler("/error");   // catches unhandled exceptions (see Advanced section)
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

---
**Next:** `06-Filters.md`
