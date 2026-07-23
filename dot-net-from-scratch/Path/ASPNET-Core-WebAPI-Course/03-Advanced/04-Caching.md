# 4. Caching

## Why cache?

Caching avoids repeating expensive work (database queries, external API calls) for data that doesn't change on every request. It's one of the highest-leverage performance improvements you can make.

## In-Memory caching

```bash
# Microsoft.Extensions.Caching.Memory is included by default in most templates
```

```csharp
builder.Services.AddMemoryCache();
```

```csharp
public class BookService : IBookService
{
    private readonly IBookRepository _repo;
    private readonly IMemoryCache _cache;

    public BookService(IBookRepository repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<IEnumerable<BookResponseDto>> GetAllAsync()
    {
        return await _cache.GetOrCreateAsync("books:all", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            var books = await _repo.GetAllAsync();
            return books.Select(b => b.ToDto()).ToList();
        }) ?? Enumerable.Empty<BookResponseDto>();
    }

    public async Task<BookResponseDto> CreateAsync(CreateBookDto dto)
    {
        var created = await _repo.CreateAsync(dto.ToEntity());
        _cache.Remove("books:all");    // invalidate stale cache on write
        return created.ToDto();
    }
}
```

**Limitation:** in-memory cache lives inside a single server's process — doesn't work if you scale to multiple server instances (each has its own separate cache, leading to inconsistency).

## Distributed caching with Redis (for multi-server deployments)

```bash
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
```

```csharp
public class BookService
{
    private readonly IDistributedCache _cache;

    public async Task<IEnumerable<BookResponseDto>> GetAllAsync()
    {
        var cached = await _cache.GetStringAsync("books:all");
        if (cached is not null)
            return JsonSerializer.Deserialize<List<BookResponseDto>>(cached)!;

        var books = (await _repo.GetAllAsync()).Select(b => b.ToDto()).ToList();

        await _cache.SetStringAsync("books:all", JsonSerializer.Serialize(books),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

        return books;
    }
}
```

All server instances share the same Redis cache, so it stays consistent across a load-balanced deployment.

## Output caching (.NET 7+, caches entire HTTP responses)

```csharp
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("BooksPolicy", policy => policy.Expire(TimeSpan.FromMinutes(2)));
});

var app = builder.Build();
app.UseOutputCache();
```

```csharp
[HttpGet]
[OutputCache(PolicyName = "BooksPolicy")]
public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetAll() =>
    Ok(await _service.GetAllAsync());
```

This is the simplest option when you want to cache a whole endpoint's response without writing any caching code manually.

## HTTP response caching headers (client/proxy-side caching)

```csharp
[HttpGet("{id}")]
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
public async Task<ActionResult<BookResponseDto>> GetById(int id) { ... }
```

This tells browsers/CDNs to cache the response for 60 seconds — useful for public, rarely-changing data (avoid for user-specific or sensitive data).

## Cache invalidation strategy — the hard part

> "There are only two hard things in Computer Science: cache invalidation and naming things."

Common strategies:
1. **Time-based expiration** (simplest) — cache expires after N minutes regardless of changes.
2. **Explicit invalidation on write** — clear/update the relevant cache key whenever the underlying data changes (shown in the example above).
3. **Cache-aside pattern** — application code checks cache first, falls back to the database on a miss, then populates the cache (the pattern used in all examples above).

Choose expiration windows based on how stale the data can safely be — a product catalog might tolerate 5 minutes; a stock ticker tolerates milliseconds.

---
**Next:** `05-API-Versioning.md`
