# 9. Performance Best Practices

## 1. Avoid N+1 query problems

```csharp
// ❌ N+1: one query for books, then one query per book for its author
var books = await _context.Books.ToListAsync();
foreach (var book in books)
{
    var author = await _context.Authors.FindAsync(book.AuthorId);   // runs N times!
}

// ✅ One query using Include
var books = await _context.Books.Include(b => b.Author).ToListAsync();
```

## 2. Project only what you need

```csharp
// ❌ Loads every column, then discards most of them
var books = await _context.Books.ToListAsync();
var titles = books.Select(b => b.Title);

// ✅ Only Title is ever pulled from the database
var titles = await _context.Books.Select(b => b.Title).ToListAsync();
```

## 3. Use AsNoTracking for read-only queries

```csharp
var books = await _context.Books.AsNoTracking().ToListAsync();
```
Skips EF Core's change-tracking bookkeeping — meaningful savings on large read-heavy endpoints.

## 4. Paginate everything that can grow unbounded

```csharp
[HttpGet]
public async Task<ActionResult<PagedResult<BookResponseDto>>> GetAll(
    [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
{
    pageSize = Math.Min(pageSize, 100);   // cap it — never trust client input

    var totalCount = await _context.Books.CountAsync();
    var items = await _context.Books
        .OrderBy(b => b.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(b => b.ToDto())
        .ToListAsync();

    return Ok(new PagedResult<BookResponseDto>(items, totalCount, page, pageSize));
}
```
Never return an unbounded `GET /api/books` on a table that could grow to millions of rows.

## 5. Compress responses

```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

app.UseResponseCompression();
```

## 6. Cache aggressively where staleness is acceptable

Covered in depth in `04-Caching.md` — the single biggest lever for reducing database load on read-heavy endpoints.

## 7. Use appropriate database indexes

```csharp
modelBuilder.Entity<Book>()
    .HasIndex(b => b.Title);

modelBuilder.Entity<Book>()
    .HasIndex(b => new { b.AuthorId, b.PublishedYear });   // composite index for common filter combos
```
Index columns you filter/sort/join on frequently. Don't over-index — every index adds write overhead.

## 8. Avoid synchronous I/O

Covered in `02-Intermediate/07-Async-Await-Best-Practices.md` — blocking calls (`.Result`, `.Wait()`) starve the thread pool under load and are one of the most common causes of production outages under traffic spikes.

## 9. Right-size your DTOs and avoid over-fetching relationships

```csharp
// ❌ Loads the entire Author and all their other books too — wasteful
var books = await _context.Books.Include(b => b.Author).ThenInclude(a => a.Books).ToListAsync();

// ✅ Only load what the response actually needs
var books = await _context.Books
    .Select(b => new BookResponseDto { Id = b.Id, Title = b.Title, AuthorName = b.Author!.Name })
    .ToListAsync();
```

## 10. Use connection pooling (usually automatic, verify it's on)

```csharp
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
```
`AddDbContextPool` reuses `DbContext` instances instead of allocating a new one per request — reduces GC pressure under high throughput.

## 11. Measure before optimizing

Use tools instead of guessing:
- **`dotnet-trace`** / **`dotnet-counters`** — CLI performance profiling tools from Microsoft.
- **Application Insights** / **OpenTelemetry** — production tracing and metrics.
- **MiniProfiler** — lightweight, drop-in profiling middleware for local dev.
- **EF Core logging** — set `LogLevel.Information` for `Microsoft.EntityFrameworkCore.Database.Command` to see every generated SQL query during development.

```json
"Logging": {
  "LogLevel": {
    "Microsoft.EntityFrameworkCore.Database.Command": "Information"
  }
}
```

## 12. Rate limiting (protects your API from abuse and overload)

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });
});

app.UseRateLimiter();
```

```csharp
[EnableRateLimiting("fixed")]
[HttpGet]
public IActionResult Get() => Ok();
```

---
**Next:** `10-Microservices-Intro.md`
