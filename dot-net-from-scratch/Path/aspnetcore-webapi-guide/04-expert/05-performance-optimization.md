# 5. Performance Optimization

## Measure before optimizing

Don't guess — profile. Useful tools:

- **`dotnet-trace`** / **`dotnet-counters`** — CLI profiling tools for live processes
- **Application Insights** / **OpenTelemetry** — distributed tracing and metrics in production
- **BenchmarkDotNet** — micro-benchmarking specific methods
- **`dotnet-monitor`** — collects diagnostics from running containers

```bash
dotnet tool install --global dotnet-counters
dotnet-counters monitor -p <PID> --counters System.Runtime,Microsoft.AspNetCore.Hosting
```

## Database performance

- **Avoid N+1 queries** — use `.Include()` for related data instead of lazy-loading in a loop.
- **Project only what you need** — `.Select(x => new Dto {...})` instead of loading full entities when you only need a few fields.
- **Use `AsNoTracking()`** for read-only queries — skips EF Core's change-tracking overhead.
- **Add indexes** on columns used in `WHERE`, `JOIN`, and `ORDER BY` clauses.
- **Paginate** large result sets — never return an unbounded `GetAll()` in production.

```csharp
[HttpGet]
public async Task<ActionResult<PagedResult<BookDto>>> GetAll(int page = 1, int pageSize = 20)
{
    var query = _context.Books.AsNoTracking().OrderBy(b => b.Id);

    var totalCount = await query.CountAsync();
    var items = await query.Skip((page - 1) * pageSize).Take(pageSize)
        .Select(b => new BookDto { Id = b.Id, Title = b.Title }).ToListAsync();

    return Ok(new PagedResult<BookDto>(items, totalCount, page, pageSize));
}
```

## Response compression

```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
});

app.UseResponseCompression();
```

Reduces payload size for JSON-heavy APIs, especially over slower networks.

## Output/response caching

Covered in `03-advanced/06-caching.md` — this is often the single biggest win for read-heavy endpoints.

## Minimize allocations on hot paths

- Prefer `IAsyncEnumerable<T>` for streaming large results instead of buffering an entire list in memory.
- Reuse `HttpClient` instances via `IHttpClientFactory` (never `new HttpClient()` per request — exhausts sockets).
- Use `System.Text.Json` (the default) over `Newtonsoft.Json` — it's faster and lower-allocation for most scenarios.

## Connection resiliency and pooling

```csharp
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(3)));
```

`AddDbContextPool` reuses `DbContext` instances across requests instead of constructing a new one every time, reducing allocation overhead under high load.

## Rate limiting (protects your API and downstream resources)

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 10;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

app.UseRateLimiter();
```

```csharp
[EnableRateLimiting("fixed")]
[HttpGet]
public async Task<IActionResult> GetAll() { ... }
```

## Load testing

Before shipping, verify behavior under load with **k6**, **Bombardier**, or **Apache JMeter**:

```bash
k6 run --vus 50 --duration 30s load-test.js
```

Watch for: rising p95/p99 latency, thread pool starvation, database connection pool exhaustion, and memory growth (possible leak) under sustained load.

## Checklist for a performance pass

- [ ] Endpoints are fully async, no blocking calls
- [ ] Large result sets are paginated
- [ ] Hot-path queries use `AsNoTracking()` and project to DTOs
- [ ] Frequently-read, rarely-changed data is cached
- [ ] Response compression enabled
- [ ] Rate limiting protects against abuse
- [ ] Load tested under realistic concurrent traffic

**Next → `06-security-hardening.md`**
